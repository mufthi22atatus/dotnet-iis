using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TaskManager
{
    public class SimpleFileLoggerProvider : ILoggerProvider
    {
        private readonly string _logPath;
        private readonly object _lock = new object();

        public SimpleFileLoggerProvider(string logPath)
        {
            _logPath = logPath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SimpleFileLogger(categoryName, _logPath, _lock);
        }

        public void Dispose()
        {
        }
    }

    public class SimpleFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logPath;
        private readonly object _lock;

        public SimpleFileLogger(string categoryName, string logPath, object @lock)
        {
            _categoryName = categoryName;
            _logPath = logPath;
            _lock = @lock;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null) return;
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null) return;

            var logLine = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [{logLevel}] [{_categoryName}] {message}";
            if (exception != null)
            {
                logLine += Environment.NewLine + exception.ToString();
            }

            lock (_lock)
            {
                try
                {
                    var actualPath = _logPath;
                    if (actualPath.Contains("-."))
                    {
                        actualPath = actualPath.Replace("-.", $"-{DateTime.UtcNow:yyyyMMdd}.");
                    }
                    else if (actualPath.EndsWith("-"))
                    {
                        actualPath = actualPath + $"{DateTime.UtcNow:yyyyMMdd}.log";
                    }

                    var dir = Path.GetDirectoryName(actualPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.AppendAllText(actualPath, logLine + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // Fail silently to prevent crashing the application on log errors
                }
            }
        }
    }
}
