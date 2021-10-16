using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using C4ImagingNetCore.Backend;
using static C4ImagingNetCore.Backend.AspectRatioAnaliser;
using System.Collections.Generic;
using Workflow.States.Kernel;
using System.Linq.Expressions;

namespace Workflow.States.Generic.Cat.Img.ByAspectRatio
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient client;
        private readonly CommandLineOptions _commandLineOptions;

        public Worker(ILogger<Worker> logger, CommandLineOptions commandLineOptions)
        {
            _logger = logger;
            _commandLineOptions = commandLineOptions;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation("The service has been stopped at " + DateTimeOffset.Now);
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service started.");
   
            // Figure out if the MAIN WorkFlow state INBOX directory exist
            if (!Directory.Exists(_commandLineOptions.Path))
            {
                _logger.LogError($"Inbox Directory \"{_commandLineOptions.Path}\" does not exist.");
                return;
            }

            // Figure out if the MAIN WorkFlow state OUTBOX directory exist
            if (!Directory.Exists(_commandLineOptions.OutputPath))
            {
                _logger.LogError($"Outbox Directory \"{_commandLineOptions.OutputPath}\" does not exist.");
                return;
            }

            // TODO:    Check for rw permissions on directories
            //          REM: This is not strictly necessary right now.

            await InitWatchersAsync(_commandLineOptions.Path, _commandLineOptions.Extensions, _commandLineOptions.ExecOrder, ProcessFileAsync);

            // Fianlly, setup the task
            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

        }

        private async Task InitWatchersAsync<T>(string workFlowInboxPath, string[] allowedFileExtensions, int workFlowExecOrder, Func<string, Task<T>> enventHandler)
        {
            // Figure out which ones are the INPUT DIRECTORIES FOR THIS WORKFLOW STATE
            // based on its EXECUTION ORDER (non-optional command line parameter).
            // (ie. where to start working)
            // some var's declarations...
            WindowsFileSystemSupport workFlowStateHelper = new WindowsFileSystemSupport(workFlowInboxPath);
            string[] inputDirectoriesForCurrentState = workFlowStateHelper.GetworkFlowStateInputDirectories(workFlowExecOrder, true);

            // Figure out if the previous state has produced output (subdirectories)
            string[] subDirectories = workFlowStateHelper.GetworkFlowStateInputDirectories(workFlowExecOrder, true);

            _logger.LogInformation("Workflow State number: {0}, of {1} active states", workFlowExecOrder, workFlowStateHelper.GetworkFlowActiveStates());
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
                    await ProcessFileAsync(e.FullPath);
                };
                foreach (var extension in allowedFileExtensions)
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
                Path = workFlowInboxPath,
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
                if (workFlowStateHelper.DiretoryPathIsTheOutputOfToWorkflowState(e.FullPath, workFlowExecOrder - 1))
                {
                    // ... and must be logged
                    _logger.LogWarning("NEW SUBCATEGORY FOUND: '" + e.Name + "' (Subdirectory created) ");
                    // Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

                // WARNING: Now, we need to restart the service (!) otherwise, new incoming files in such new category 
                // will not get processed, because thre are not event handler for them.
                // This situation is going to occur very often at the first workflow executions, until
                // the workflow have been "learned" all the possible categories:

                // if new folder has been created on the output folder of ANY previous state
                // then that is INDIRECTLY RELEVANT FOR THIS WORKFLOW NODE...
                if (workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) == (workFlowExecOrder - 1))
                {
                    _logger.LogWarning("NEW CATEGORY CREATED in MY level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

                if (workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) < (workFlowExecOrder - 1))
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
                if (workFlowStateHelper.DiretoryPathIsTheOutputOfToWorkflowState(e.FullPath, workFlowExecOrder - 1))
                {
                    // ... and must be logged
                    _logger.LogWarning("NEW SUBCATEGORY DELETED: '" + e.Name + "' (Subdirectory created) ");
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

                // WARNING: Now, we need to restart the service (!) otherwise, we have an unused directory watcher working

                // if a folder has been deleted on the output folder of ANY previous state
                // then that is INDIRECTLY RELEVANT FOR THIS WORKFLOW NODE...
                if (workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) == (workFlowExecOrder - 1))
                {
                    _logger.LogWarning("NEW CATEGORY DELETED in MY level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

                if (workFlowStateHelper.GetWorkFlowStateOrderNumberFromDirectoryPath(e.FullPath) < (workFlowExecOrder - 1))
                {
                    _logger.LogWarning("NEW CATEGORY DELETED in higher level: " + e.FullPath);

                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }


            };

            // and fianlly, activate the subfolder watcher
            newFolderWatcher.EnableRaisingEvents = true;
        }

        private async Task<int> ProcessAllExistingFilesAsync()
        {
            int count = 0;
            foreach (var extension in _commandLineOptions.Extensions)
            {
                foreach (var filePath in Directory.EnumerateFiles(_commandLineOptions.Path, $"*.{extension}", SearchOption.TopDirectoryOnly))
                {
                    if (await ProcessFileAsync(filePath))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private Task<bool> ProcessFileAsync(string filePath)
        {
            ImageCategorizationResults imgCategoryzationResults = new ImageCategorizationResults();

            try
            {
                _logger.LogInformation("AN IMAGE HAS BEEN RECEIVED:" + filePath);

                // call the Backend dll and ctegorize the received file
                imgCategoryzationResults = GetImageAspectRatio(filePath);

                // analise the categaroization results and log them
                foreach (ImageCategorizationResult imgCategoryzationResult in imgCategoryzationResults.List)
                {
                    // Image analysis, exception handling and logging
                    if (imgCategoryzationResult.HasException)
                    {
                        _logger.LogWarning("Exception: " + imgCategoryzationResult.ExceptionDescription);
                    }
                    else
                    {
                        _logger.LogInformation("AN IMAGE HAS BEEN CATEGORIZED BY ASPECT RATIO: " + Path.GetFileName(filePath) + " IS " + imgCategoryzationResult.ImageCategory, imgCategoryzationResult.LogId);
                    }
                }

                // proceed to move the images to its correspondent folders based on
                // its category, by analisysng the ImageCategorizationResults
                foreach (ImageCategorizationResult imgCategoryzationResult in imgCategoryzationResults.List)
                {
                    // Create folder
                    if (!Directory.Exists(imgCategoryzationResult.ImageCategory))
                    {
                        Directory.CreateDirectory(imgCategoryzationResult.ImageCategory).ToString();
                    }

                    Kernel.WindowsFileSystemSupport.MoveFileToFolder(filePath, imgCategoryzationResult.ImageCategory);

                    // and log it
                    _logger.LogInformation("AN IMAGE HAS BEEN MOVED: " + filePath + " TO .\\" + imgCategoryzationResult.ImageCategory);
                }

                // now, the categorized images are ready for another and more accurate
                // categorization performed by another background worker like this one, which can live in the
                // same server on in a remote one.

                // NEXT, PLEASE! 
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError( "FILE EXCEPTION: " + ex.Message + ": FILE NAME: " + Path.GetFileName(filePath), $"Error when processing file {filePath}");
                return Task.FromResult(false);
            }

        }

    }
}
