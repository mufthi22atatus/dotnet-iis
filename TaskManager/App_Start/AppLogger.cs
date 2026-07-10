using Microsoft.Extensions.Logging;

namespace TaskManager
{
    public static class AppLogger
    {
        /// <summary>
        /// Creates an ILogger instance using the globally configured LoggerFactory.
        /// </summary>
        public static ILogger<T> Create<T>()
        {
            return MvcApplication.LoggerFactory?.CreateLogger<T>();
        }
    }
}
