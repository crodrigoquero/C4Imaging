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

namespace C4imagingNetCore.Workflow.Srv
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

            if (!Directory.Exists(_commandLineOptions.Path))
            {
                _logger.LogError($"Directory \"{_commandLineOptions.Path}\" does not exist.");
                return;
            }

            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

             _logger.LogInformation($"Listening for images created in \"{_commandLineOptions.Path}\"...");

            using FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = _commandLineOptions.Path
            };

            foreach (var extension in _commandLineOptions.Extensions)
            {
                watcher.Filters.Add($"*.{extension}");
            }

            //adding event handler to "created" event (using lambda expression
            watcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                await Task.Delay(1000);
                await ProcessFileAsync(e.FullPath);
            };

            watcher.EnableRaisingEvents = true; //activating the fieWatcher

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
                // categorization performed by another background worker like this, in the
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
