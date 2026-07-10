using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Targets;
using Serilog;

namespace TaskManager
{
    public static class LoggingConfig
    {
        /// <summary>
        /// Picks the logging backend: "nlog", "serilog", or the default Microsoft.Extensions.Logging
        /// (ILogger) provider (SimpleFileLoggerProvider) for anything else/unset.
        /// </summary>
        public const string ProviderEnvVar = "LOGGING_PROVIDER";

        public static string ActiveProvider { get; private set; }

        public static void Configure(ILoggingBuilder builder)
        {
            var logRootCfg = ConfigurationManager.AppSettings["App:LogRoot"] ?? "~/Logs";
            var logRoot = logRootCfg.StartsWith("~/")
                ? HostingEnvironment.MapPath(logRootCfg)
                : logRootCfg;

            if (!string.IsNullOrEmpty(logRoot) && !Directory.Exists(logRoot))
            {
                Directory.CreateDirectory(logRoot);
            }

            var provider = (Environment.GetEnvironmentVariable(ProviderEnvVar) ?? "ilogger").Trim().ToLowerInvariant();

            switch (provider)
            {
                case "nlog":
                    ActiveProvider = "nlog";
                    ConfigureNLog(builder, logRoot);
                    break;
                case "serilog":
                    ActiveProvider = "serilog";
                    ConfigureSerilog(builder, logRoot);
                    break;
                default:
                    ActiveProvider = "ilogger";
                    var logPath = Path.Combine(logRoot ?? Path.GetTempPath(), "taskmanager-.log");
                    builder.AddProvider(new SimpleFileLoggerProvider(logPath));
                    break;
            }

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    AppLogger.Create<object>()?.LogCritical(ex, "AppDomain unhandled exception");
            };
        }

        private static void ConfigureSerilog(ILoggingBuilder builder, string logRoot)
        {
            var logPath = Path.Combine(logRoot ?? Path.GetTempPath(), "taskmanager-.log");

            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            builder.AddSerilog(serilogLogger, dispose: true);
        }

        private static void ConfigureNLog(ILoggingBuilder builder, string logRoot)
        {
            var nlogConfig = new NLog.Config.LoggingConfiguration();

            var fileTarget = new FileTarget("file")
            {
                FileName = Path.Combine(logRoot ?? Path.GetTempPath(), "taskmanager.log"),
                ArchiveFileName = Path.Combine(logRoot ?? Path.GetTempPath(), "taskmanager-{#}.log"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveDateFormat = "yyyyMMdd",
                MaxArchiveFiles = 14,
                Layout = "[${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff}] [${level:uppercase=true}] [${logger}] ${message}${onexception:${newline}${exception:format=tostring}}"
            };

            nlogConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, fileTarget);

            builder.AddNLog(nlogConfig);
        }
    }
}
