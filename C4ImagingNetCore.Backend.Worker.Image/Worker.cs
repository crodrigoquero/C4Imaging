using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace C4ImagingNetCore.Backend.Worker.Image
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        HttpClient client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation("The service has been stopped at " + DateTime.Now.ToString());
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var result = await client.GetAsync("https://www.puregym.com");

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The website is up. Satus code {StatusCode}", result.StatusCode);
                }
                else
                {
                    _logger.LogError("The website is down. Status code {StatusCode}", result.StatusCode);
                }
                //await Task.Delay(60*1000, stoppingToken); // EVERY MINUTE: maybe is better to change numero values into     CONSTANTS
                await Task.Delay(5000, stoppingToken); // EVERY 5 SECONDS: maybe is better to change numero values into     CONSTANTS


            }
        }
    }
}
