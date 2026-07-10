using System;
using System.Configuration;
using Microsoft.Extensions.Logging;

namespace TaskManager.Background
{
    /// <summary>
    /// Lightweight in-process scheduler. Drives jobs on System.Threading.Timer ticks.
    /// Real apps should look at Hangfire / Quartz; this is intentionally tiny so the sample
    /// doesn't take on extra deps but still produces background-task telemetry.
    /// </summary>
    public static class BackgroundJobHost
    {
        private static TaskReminderJob _reminder;
        private static StaleTaskCleanupJob _cleanup;

        public static void Start()
        {
            var enabled = bool.TryParse(ConfigurationManager.AppSettings["Jobs:Enabled"], out var e) ? e : true;
            if (!enabled)
            {
                AppLogger.Create<object>()?.LogInformation("Background jobs disabled by config");
                return;
            }

            var reminderSecs = int.Parse(ConfigurationManager.AppSettings["Jobs:ReminderIntervalSeconds"] ?? "60");
            var cleanupSecs = int.Parse(ConfigurationManager.AppSettings["Jobs:CleanupIntervalSeconds"] ?? "300");

            _reminder = new TaskReminderJob(TimeSpan.FromSeconds(reminderSecs));
            _cleanup = new StaleTaskCleanupJob(TimeSpan.FromSeconds(cleanupSecs));

            _reminder.Start();
            _cleanup.Start();

            AppLogger.Create<object>()?.LogInformation("Background jobs started (reminder={R}s, cleanup={C}s)", reminderSecs, cleanupSecs);
        }

        public static void Stop()
        {
            _reminder?.Stop();
            _cleanup?.Stop();
        }
    }
}
