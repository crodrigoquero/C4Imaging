using C4.ImaginingNetCore.Notifier.Logging;
using C4.ImaginingNetCore.Notifier.Logging.Response;
using C4ImagingNetCore.Backend;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using static C4ImagingNetCore.Backend.AspectRatioAnaliser;

namespace C4ImagingNetCore.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            // var declaration
            ImageCategorizationResults imgCategoryzationResults = new ImageCategorizationResults();
            var services = new ServiceCollection();

            // proceed to configure app services
            ConfigureServices(services);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            Notifier app = serviceProvider.GetService<Notifier>();
            Status status; //holds log operations result

            // logging APP STARTUP
            status = app.SendNotification("APPLICATION STARTED at "  + DateTime.Now);

            // enforcing app prerequsites/business rules
            if (args.Count() == 0)
            {
                status = app.SendError("Please provide a file name (do not include file path)");
                Environment.Exit(0);
            }

            // logging RECEIVED IMAGE
            status = app.SendNotification("File '" + args[0] + "' received");

            // Prepare analysis execution parameters
            string imagePath = "./img/" + args[0]; // I take this value from console parameters

            imgCategoryzationResults = GetImageAspectRatio(imagePath);

            foreach(ImageCategorizationResult imgCategoryzationResult in imgCategoryzationResults.List)
            {
                // Image analysis, exception handling and logging
                if (imgCategoryzationResult.HasException)
                {
                    app.SendWarning("Exception: " + imgCategoryzationResult.ExceptionDescription);
                }
                else
                {
                    app.SendNotification("It seems that the current image has " + imgCategoryzationResult.ImageCategory + " aspect ratio", imgCategoryzationResult.LogId);
                }
            }
           
            status = app.SendNotification("APPLICATION ENDED at " + DateTime.Now);
        }



        #region App Configuration
        /// <summary>
        /// Just configures the Logger service
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
            .AddSingleton<Notifier>(); // siglenton: same logger instance for the whole app (to avoid memory leaks, btw)

        }


        #endregion


    }
}
