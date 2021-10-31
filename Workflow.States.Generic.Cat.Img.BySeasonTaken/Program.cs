using C4ImagingNetCore.Backend.CommandLine.Img.Cat;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace Workflow.States.Generic.Cat.Img.BySeasonTaken
{
    public class Program
    {
        /// <summary>
        /// When running as service, the command line args are passed as parameters when the service is created
        /// my using the "SC" command in Windows PowerShell.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task<int> Main(string[] args)
        {
            // using SeriLog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"C:\Temp\workerservice\LogFile.txt")
                .CreateLogger();

            // calling commandLineParser
            return await Parser.Default.ParseArguments<StandardOptions>(args)
                .MapResult(async (StandardOptions standardOptions) =>
                {
                    // We have the parsed arguments, so let's just pass them down
                    await CreateHostBuilder(args, standardOptions).Build().RunAsync();
                    return 0;
                },
                errs => Task.FromResult(-1)); // Invalid arguments
                                              // Task.FromResult() creates a finished Task that holds a value in its Result property.
                                              // It allows you to create a pre-computed task.
                                              // "FromResult" is useful when you call a non async function as a return value of a task
        }

        public static IHostBuilder CreateHostBuilder(string[] args, StandardOptions standardOptions)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService() // nuget pkg "Microsoft.Extensions.Hosting.WindowsServices"
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton(standardOptions);
                })
                .UseSerilog();
        }

    }
}
