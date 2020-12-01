using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Bidirectional.Demo.Client.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using Serilog.Core;

namespace Bidirectional.Demo.Client
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(Environment.ProcessorCount * 8, 1000);
            ThreadPool.SetMaxThreads(Environment.ProcessorCount * 128, 1000);
                        
            Log.Logger = CreateLogger(args);

            try
            {
                Log.ForContext<Program>().Information("Starting up Bidirectional Demo Client");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>().Fatal(ex, "Starting up failed for Bidirectional Demo Client");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureServices(RootConfigurator.ConfigureServices);
        }
        
        private static Logger CreateLogger(string[] args)
        {
            var environment = GetEnvironment();
            var configuration = GetConfiguration();
            var basePath = GetBasePath();
            var outputTemplate =
                "{Timestamp:HH:mm:ss.fff} [{Level:u4}] {ThreadName}#{ThreadId:000} ({SourceContext}) {Message}{NewLine}{Exception}";

            return new LoggerConfiguration()
                .WriteTo.Async(asyncConfig =>
                {
                    asyncConfig.Debug(outputTemplate: outputTemplate);
                    asyncConfig.Console(outputTemplate: outputTemplate);
                    asyncConfig.File(
                        path: Path.Join(basePath, "logs\\Bidirectional.Demo.Client.-.log"),
                        outputTemplate: outputTemplate,
                        retainedFileCountLimit: 3,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: false,
                        shared: true
                    );
                }, bufferSize: 100_000, blockWhenFull:true)
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .CreateLogger();

            string GetEnvironment()
            {
                return new ConfigurationBuilder()
                           .AddCommandLine(args)
                           .AddEnvironmentVariables()
                           .Build()
                           .GetValue<string>("Environment") 
                       ?? (
                           DebugDetector.AreWeInDebugMode
                               ? Environments.Development
                               : Environments.Production
                       );
            }

            IConfigurationRoot GetConfiguration()
            {
                return new ConfigurationBuilder()
                    .SetBasePath(GetBasePath())
                    .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                    .AddJsonFile($"appsettings.{environment}.json", reloadOnChange: true, optional: true)
                    .AddJsonFile($"appsettings.{Environment.MachineName}.json", reloadOnChange: true, optional: true)
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();
            }

            string GetBasePath()
            {
                // When running as a Windows Service, the current directory will be C:\Windows\System32, so we must switch to the assembly directory
                return WindowsServiceHelpers.IsWindowsService()
                    ? AppContext.BaseDirectory
                    : Directory.GetCurrentDirectory();
            }
        }
    }

    internal static class DebugDetector
    {
        public static bool AreWeInDebugMode;

        static DebugDetector()
        {
            YesWeAre();
        }

        /**
         * This method will be stripped out by the compiler when this project is built in Release mode
         */
        [Conditional("DEBUG")]
        private static void YesWeAre()
        {
            AreWeInDebugMode = true;
        }
    }
}