using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Entities;
using TaskManager.Services;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;

namespace TaskManager.Background
{
    /// <summary>
    /// Polls the DB for tasks due in the next 24h and fires a notification once per task.
    /// Generates: DB span (query), notification span (per task), summary log per tick.
    /// </summary>
    public class TaskReminderJob
    {
        private readonly TimeSpan _interval;
        private Timer _timer;
        private int _running; // CAS guard so overlapping ticks don't pile up

        public TaskReminderJob(TimeSpan interval)
        {
            _interval = interval;
        }

        public void Start()
        {
            _timer = new Timer(async _ => await TickAsync(), null, _interval, _interval);
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private async Task TickAsync()
        {
            if (Interlocked.Exchange(ref _running, 1) == 1) return;
            var sw = Stopwatch.StartNew();
            try
            {
                using (var ctx = new AppDbContext())
                {
                    var threshold = DateTime.UtcNow.AddHours(24);
                    var due = await ctx.Tasks
                        .Include(t => t.AssignedTo)
                        .Where(t => t.DueDate != null
                                    && t.DueDate <= threshold
                                    && t.Status != TaskStatus.Done
                                    && t.Status != TaskStatus.Cancelled)
                        .Take(100)
                        .ToArrayAsync();

                    var notifier = new NotificationService();
                    foreach (var t in due)
                    {
                        try
                        {
                            await notifier.NotifyTaskDueSoonAsync(t);
                        }
                        catch (Exception ex)
                        {
                            AppLogger.Create<TaskReminderJob>()?.LogWarning(ex, "Reminder notify failed for task {Id}", t.Id);
                        }
                    }

                    AppLogger.Create<TaskReminderJob>()?.LogInformation("ReminderJob tick: {Count} tasks reminded in {Ms}ms",
                        due.Length, sw.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Create<TaskReminderJob>()?.LogError(ex, "ReminderJob tick failed");
            }
            finally
            {
                Interlocked.Exchange(ref _running, 0);
            }
        }
    }
}
