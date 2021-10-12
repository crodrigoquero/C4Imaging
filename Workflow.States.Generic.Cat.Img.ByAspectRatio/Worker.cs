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

namespace Workflow.States.Generic.Cat.Img.ByAspectRatio
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient client;
        private readonly CommandLineOptions _commandLineOptions;

        private enum WorkerStartupInputType { standard=1, multicategory}
        WorkerStartupInputType _workerStartupInputType = WorkerStartupInputType.standard;

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
            _logger.LogInformation("Workflow State number: {0}.", _commandLineOptions.ExecOrder);

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

            // Figure out which ones are the INPUT DIRECTORIES FOR THIS WORKFLOW STATE
            // based on its EXECUTION ORDER (non-optional command line parameter).
            // (ie. where to start working)
            // some var's declarations...
            WorkFlowStateHelper workFlowStateHelper = new WorkFlowStateHelper(_commandLineOptions.Path);
            string[] inputDirectoriesForCurrentState = workFlowStateHelper.GetworkFlowStateInputDirectories(_commandLineOptions.ExecOrder, true);


            // CRUCIAL: PROCESS PRE-EXISTING/REMAINING FILES FROM PREVIOUS EXECUTIONS
            string[] fileEntries = Directory.GetFiles(_commandLineOptions.Path);
            foreach (string fileEntry in fileEntries) // get all the files from the inbox ...
            {
                // and process them, one by one
                // BUT IF THE WORKFLOW STATE ORDER IS GREATER THAN 1, MOVE THEM BACK TO THE INBOX 
               await ProcessFileAsync(fileEntry);
            }

            ///var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


            // start refactoring here



            // Figure out if the previous state has produced output (subdirectories)
            ///////////////////////////////////////// DirectoryInfo directory = new DirectoryInfo(_commandLineOptions.Path);
            string[] subDirectories = workFlowStateHelper.GetworkFlowStateInputDirectories(_commandLineOptions.ExecOrder); //directory.GetDirectories();

            _logger.LogInformation(_commandLineOptions.Path + " has "+ subDirectories.Length + " subdirectories:" );
 
            // we need to setup the type of data entry for this service based on if the inbox folder
            // has subdirectories (the possible input types can be diverse) or not
            if (subDirectories.Length > 0) _workerStartupInputType = WorkerStartupInputType.multicategory;
            _logger.LogInformation("Assumend " + _workerStartupInputType.ToString() + " input");

            // root directory wactcher
            // It is necessary to monitor folder creation everywhere. Each workflow node must
            // consider only events behind its own level (from father's folders).
            using FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = _commandLineOptions.Path,
                IncludeSubdirectories = true // important to be able to re-start the service
            };

            //the watcher is for monitor new directories created in the root directory
            using FileSystemWatcher subFolderWatcher = new FileSystemWatcher
            {
                Path = _commandLineOptions.Path,
                IncludeSubdirectories = false // important to avoid file IO exceptions
            };

            // setup the watcher to watch for new directories only...
            subFolderWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName;
            
            // ... and assign an event handler to it
            subFolderWatcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                await Task.Delay(1000); // instead using delays, here we can implement some retry logic
                _logger.LogWarning("NEW SUBCATEGORY FOUND: '" + e.Name +  "' (Subdirectory created) ");

                // WARNING: Now, we need to restart the service (!) otherwise, new incoming files in such new category 
                // will not get processed, because thre are not event handler for them.
                // This situation is going to occur very often at the first workflow executions, until
                // the workflow have been "learned" all the possible categories:
                if(_workerStartupInputType == WorkerStartupInputType.standard)
                {
                    // YOU CAN COMMENT THE FOLLOWING LINE IN DEBUG TIME (to make your life eassier):
                    Environment.Exit(1); // CRUCIAL: this, will make the service to restart automatically
                }

            };

            // and fianlly, activate the subfolder watcher
            subFolderWatcher.EnableRaisingEvents = true;

            watcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                await Task.Delay(1000);
                await ProcessFileAsync(e.FullPath);
            };

            // create a directory watchers list
            List<FileSystemWatcher> watchersList = new List<FileSystemWatcher>();
            foreach (string dir in subDirectories)
            {
                watchersList.Add(new FileSystemWatcher { Path = dir, IncludeSubdirectories = false });
            };

            //add file extensions filter and an eventhandler to each subfolder watcher
            foreach (FileSystemWatcher watcherItem in watchersList)
            {
                foreach (var extension in _commandLineOptions.Extensions)
                {
                    watcherItem.Filters.Add($"*.{extension}");
                }

                watcherItem.Created += async (object sender, FileSystemEventArgs e) =>
                {
                    await Task.Delay(1000);
                    await ProcessFileAsync(e.FullPath);
                };

                _logger.LogInformation($"Listening for images created in \"{watcherItem.Path}\"...");
                watcherItem.EnableRaisingEvents = true; //activating the fieWatcher for each subdirectoy
            };

            // setup the kind of input we are going to receive
            _workerStartupInputType = WorkerStartupInputType.standard;

            foreach (var extension in _commandLineOptions.Extensions)
            {
                watcher.Filters.Add($"*.{extension}");
            }

            // Proceed to activate the fieWatcher for root subdirectory
            watcher.EnableRaisingEvents = true; 
            _logger.LogInformation($"Listening for images created in \"{_commandLineOptions.Path}\"...");

            // Fianlly, setup the task
            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

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

                    MoveFileToFolder(filePath, imgCategoryzationResult.ImageCategory);

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

        private void MoveFileToFolder(string filePath, string folderName)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var destinationDirectory = Path.Combine(directory, folderName);

            Directory.CreateDirectory(destinationDirectory);

            try
            {
                File.Move(filePath, Path.Combine(destinationDirectory, fileName), false);
            }
            catch (Exception ex)
            {
                File.Delete(filePath);
                throw ex; // passing the exception to higher level (for logging it)
            }

         }

    }
}
