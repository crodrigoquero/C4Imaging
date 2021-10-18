using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workflow.States.Kernel.Models;

namespace Workflow.States.Kernel
{
    /// <summary>
    /// Initialises the workflow node (inbox diirectory watchers) and reports its status and processing skill
    /// </summary>
    public class Setup
    {
        private readonly ILogger _logger;

        private readonly string _workFlowInboxPath;
        private readonly string[] _allowedFileExtensions;
        private readonly int _workFlowExecOrder;

        WindowsFileSystemSupport _workFlowStateHelper;

        // workflow state status fields
        public Status Status = new Kernel.Models.Status();

        // workflow skill (plugin)
        public Skill Skill; // still no used (comming soon; to use only when plugin get loaded). 


        // Constructor
        public Setup(ILogger logger, string workFlowInboxPath, string[] allowedFileExtensions, int workFlowExecOrder)
        {
            _logger = logger;

            _workFlowInboxPath = workFlowInboxPath;
            _allowedFileExtensions = allowedFileExtensions;
            _workFlowExecOrder = workFlowExecOrder;

            Status.ManagedDirectories = new List<string>();

            _workFlowStateHelper = new WindowsFileSystemSupport(_workFlowInboxPath);

        }
        public async Task InitWatchersAsync<T>(Func<string, Task<T>> enventHandler)
        {
            // Figure out which ones are the INPUT DIRECTORIES FOR THIS WORKFLOW STATE
            // based on its EXECUTION ORDER (non-optional command line parameter).
            // (ie. where to start working)
            // some var's declarations...
             string[] inputDirectoriesForCurrentState = _workFlowStateHelper.GetworkFlowStateInputDirectories(_workFlowExecOrder, true);

            // Figure out if the previous state has produced output (subdirectories)
            string[] subDirectories = _workFlowStateHelper.GetworkFlowStateInputDirectories(_workFlowExecOrder, true);

            _logger.LogInformation("Workflow State number: {0}, of {1} active states", _workFlowExecOrder, _workFlowStateHelper.GetworkFlowActiveStates());
            _logger.LogInformation("I do have " + subDirectories.Length + " entry point-s");


            // ADD, CONFIGURE AND SET READY ALL FILE WATCHERS...
            List<FileSystemWatcher> fileWatchers = new List<FileSystemWatcher>();
            foreach (string dir in subDirectories)
            {
                // CRUCIAL: First of all, we must process remaining files from past executions
                // which got frozen when the service was restarted
                DirectoryInfo d = new DirectoryInfo(@dir);
                FileInfo[] Files = d.GetFiles(); //Getting files

                // proceesing the remaining files
                foreach (FileInfo file in Files)
                {
                    //await eventHandler(file.FullName);
                    await enventHandler(file.FullName);
                }

                // the we can continue setting up the watchers....
                FileSystemWatcher fileWatcher = new FileSystemWatcher
                {
                    Path = dir,
                    IncludeSubdirectories = false // it is CRITICAL to be able to restart the service when new categories can potentially arise
                };

                // asign eventHandler to each FILE WATCHER
                fileWatcher.Created += async (object sender, FileSystemEventArgs e) =>
                {
                    await Task.Delay(1000);
                    await enventHandler(e.FullPath);

                    // update list of WATCHED DIRECTORIES...
                    Status.ManagedDirectories.Add(e.FullPath);

                    // ... and this WorkFlow State STATUS (CONFIGURED YES/NO) 
                    Status.Configured = true;
                };
                foreach (var extension in _allowedFileExtensions) // here is how we discriminate file types
                {
                    fileWatcher.Filters.Add($"*.{extension}");
                }

                // Proceed to activate the fieWatcher for root subdirectory
                fileWatcher.EnableRaisingEvents = true;

                _logger.LogInformation($"Listening for images created in \"{dir}\"...");


                fileWatchers.Add(fileWatcher);
            }


            // WORKFLOW ROOT DIRECTORY WATCHER
            // It is necessary to monitor folder creation & deletion everywhere. 
            // Once a new folder creation is detected, this workflow must be restarted to be able
            // to setup ncessary watchs for these new folders, otherwise the files received on these new 
            // folders will not get processed.
            // the watcher is for monitor new directories created from the workflow root directory
            // (including subdirectories)
            FileSystemWatcher newFolderWatcher = new FileSystemWatcher
            {
                Path = _workFlowInboxPath,
                IncludeSubdirectories = true, // important to be able to detect any new folder everywhere

                // setup the watcher to watch for new directories only...
                NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName

            };

            // ... and assign an event handler to it
            newFolderWatcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                await Task.Delay(1000); // instead using delays, here we can implement some retry logic

                // if new folder has been created on the output folder of the previous state
                // then that is DIRECTLY RELEVANT FOR THIS WORKFLOW NODE...

                // WARNING: Now, we need to restart the service (!) otherwise, new incoming files in such new category 
 
                // will not get processed, because thre are not event handler for them.
                // This situation is going to occur very often at the first workflow executions, until
                // the workflow have been "learned" all the possible categories:

                // if new folder has been created on the output folder of ANY previous state
                // then that is INDIRECTLY RELEVANT FOR THIS WORKFLOW NODE...
                if (_workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) == (_workFlowExecOrder - 1))
                {
                    _logger.LogWarning("NEW CATEGORY CREATED in MY level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

                if (_workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) < (_workFlowExecOrder - 1))
                {
                    _logger.LogWarning("NEW CATEGORY CREATED in higher level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

            };


            // WORKFLOW ROOT DIRECTORY WATCHER FOR DELETED FOLDERS
            newFolderWatcher.Deleted += async (object sender, FileSystemEventArgs e) =>
            {
                await Task.Delay(1000); // instead using delays, here we can implement some retry logic

                // if a folder has been deleted on the output folder of the previous state
                // then that is DIRECTLY RELEVANT FOR THIS WORKFLOW NODE...

                // WARNING: Now, we need to restart the service (!) otherwise, we have an unused directory watcher working

                // if a folder has been deleted on the output folder of ANY previous state
                // then that is INDIRECTLY RELEVANT FOR THIS WORKFLOW NODE...
                if (_workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) == (_workFlowExecOrder - 1))
                {
                    _logger.LogWarning("CATEGORY DELETED in MY level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

                if (_workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) < (_workFlowExecOrder - 1))
                {
                    _logger.LogWarning("CATEGORY DELETED in higher level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }


            };

            // and fianlly, activate the subfolder watcher
            newFolderWatcher.EnableRaisingEvents = true;
        }

    }
}
