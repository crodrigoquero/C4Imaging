using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using CommandLine;

namespace Workflow.States.Generic.Cat.Img.ByAspectRatio
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
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions commandLineOptions) =>
                {
                    // We have the parsed arguments, so let's just pass them down
                    await CreateHostBuilder(args, commandLineOptions).Build().RunAsync();
                    return 0;
                },
                errs => Task.FromResult(-1)); // Invalid arguments
                                              // Task.FromResult() creates a finished Task that holds a value in its Result property.
                                              // It allows you to create a pre-computed task.
                                              // "FromResult" is useful when you call a non async function as a return value of a task
        }

        public static IHostBuilder CreateHostBuilder(string[] args, CommandLineOptions commandLineOptions)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService() // nuget pkg "Microsoft.Extensions.Hosting.WindowsServices"
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton(commandLineOptions);
                })
                .UseSerilog();
        }

    }
}
