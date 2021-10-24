using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using C4ImagingNetCore.Backend;
using static C4ImagingNetCore.Backend.ImageAnaliser;
using Workflow.States.Kernel.IO.FileSys.Win;
using System.Collections.Generic;

namespace Workflow.States.Generic.Cat.Img.ByLocation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CommandLineOptions _commandLineOptions;
        private Setup _workFlowStateKernel;

        public Worker(ILogger<Worker> logger, CommandLineOptions commandLineOptions)
        {
            _logger = logger;
            _commandLineOptions = commandLineOptions;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
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

            // TODO: LOAD PLUGIN / WORKFLOW STATE SKILL here!

            // CONFIGURATION OF THE WORKFLOW STATE PROCESS
            _workFlowStateKernel = new Setup(_logger, _commandLineOptions.Path, _commandLineOptions.Extensions, _commandLineOptions.ExecOrder);
            await _workFlowStateKernel.InitWatchersAsync(ProcessFileAsync);

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
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();
 
            try
            {
                _logger.LogInformation("AN IMAGE HAS BEEN RECEIVED:" + filePath);

                // call the Backend dll and categorize the received file
                // In this case, an api key for google geocode api is required; such key
                // must reside in the config file
                imgCategoryzationResults = GetImageCountryTaken(filePath, "[apikey]");

                // test
                foreach (ImageCategorizationResult imgCategoryzationResult in imgCategoryzationResults)
                {
                    _logger.LogInformation("Latitude: " + imgCategoryzationResult.Latitude.ToString() + 
                        " Longitude:" + imgCategoryzationResult.Longitude.ToString() + 
                        " Country: " + imgCategoryzationResult.ImageCategory);
                }

                // analise the categaroization results and log them
                foreach (ImageCategorizationResult imgCategoryzationResult in imgCategoryzationResults)
                {
                    // Image analysis and logging
                    _workFlowStateKernel.Status.TotalFiles += 1; //update session file counter
                    _logger.LogInformation("AN IMAGE HAS BEEN CATEGORIZED BY LOCATION: " + Path.GetFileName(filePath) + " WAS TAKEN IN " + imgCategoryzationResult.ImageCategory, imgCategoryzationResult.LogId);
                }

                // proceed to move the images to its correspondent folders based on
                // its category, by analisysng the ImageCategorizationResults
                foreach (ImageCategorizationResult imgCategoryzationResult in imgCategoryzationResults)
                {
                    // Create folder
                    if (!Directory.Exists(imgCategoryzationResult.ImageCategory))
                    {
                        Directory.CreateDirectory(imgCategoryzationResult.ImageCategory).ToString();
                    }

                    WinFileSys.MoveFileToFolder(filePath, imgCategoryzationResult.ImageCategory);

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
                _logger.LogError("FILE EXCEPTION. " + ex.Message + ": FILE NAME: " + Path.GetFileName(filePath), $"Error when processing file {filePath}");
                return Task.FromResult(false);
            }

        }

    }
}
