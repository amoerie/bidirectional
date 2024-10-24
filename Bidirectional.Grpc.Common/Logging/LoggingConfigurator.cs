using System;
using System.IO;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using Serilog.Events;

namespace Bidirectional.Grpc.Common.Logging;

public static class LoggingConfigurator
{
    /// <summary>
    /// Returns a bootstrap logger that will only exist shortly and be reconfigured after startup
    /// See https://nblumhardt.com/2020/10/bootstrap-logger/
    /// </summary>
    public static ILogger CreateLogger()
    {
        const string outputTemplate = "[{Timestamp:u} {Level:u3} {RequestId} #{ThreadId:000} {SourceContext}]{NewLine}{Message}{NewLine}{Exception}";
        const string consoleOutputTemplate = "[{Timestamp:HH:mm:ss.fffZ} {Level:u3} {RequestId} #{ThreadId:000} {SourceContext}]{NewLine}{Message}{NewLine}{Exception}";
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Information)
            .Enrich.FromLogContext();

        // Only write to <default-files-dir>/Startup.log if the <default-files-dir> folder exists
        // When running as a Windows Service, the current directory will be C:\Windows\System32, so we must switch to the assembly directory
        var basePath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : Directory.GetCurrentDirectory();
        const string filesDirectory = "./logs";
        var filesBasePath = Path.IsPathRooted(filesDirectory)
            ? filesDirectory
            : Path.Join(basePath, filesDirectory);
        if (Directory.Exists(filesBasePath))
        {
            var startupLogFileName = "Startup.log";
            var startupLogFilePath = Path.Join(filesBasePath, startupLogFileName);
            loggerConfiguration.WriteTo.File(startupLogFilePath, outputTemplate: outputTemplate, shared: true);
        }

        // Only write to console when a console is shown
        if (Environment.UserInteractive)
        {
            loggerConfiguration.WriteTo.Console(outputTemplate: consoleOutputTemplate);
        }

        return loggerConfiguration.CreateLogger();
    }
}
