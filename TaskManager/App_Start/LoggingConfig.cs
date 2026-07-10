using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskManager
{
    public static class LoggingConfig
    {
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

            var logPath = Path.Combine(logRoot ?? Path.GetTempPath(), "taskmanager-.log");

            builder.AddProvider(new SimpleFileLoggerProvider(logPath));

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    AppLogger.Create<object>()?.LogCritical(ex, "AppDomain unhandled exception");
            };
        }
    }
}
