using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Entities;

namespace TaskManager.Background
{
    /// <summary>
    /// Archives audit log rows older than 90 days. Demonstrates batched DB work + file I/O
    /// (writes a CSV summary to the Logs dir each cycle).
    /// </summary>
    public class StaleTaskCleanupJob
    {
        private readonly TimeSpan _interval;
        private Timer _timer;
        private int _running;

        public StaleTaskCleanupJob(TimeSpan interval) { _interval = interval; }

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
                    var cutoff = DateTime.UtcNow.AddDays(-90);
                    var old = await ctx.AuditLogs
                        .Where(a => a.CreatedAt < cutoff)
                        .OrderBy(a => a.CreatedAt)
                        .Take(500)
                        .ToArrayAsync();

                    if (old.Length == 0)
                    {
                        AppLogger.Create<StaleTaskCleanupJob>()?.LogInformation("CleanupJob: nothing to archive ({Ms}ms)", sw.ElapsedMilliseconds);
                        return;
                    }

                    var logRoot = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs") ?? System.IO.Path.GetTempPath();
                    var archivePath = System.IO.Path.Combine(logRoot, $"audit-archive-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
                    using (var sw2 = new System.IO.StreamWriter(archivePath))
                    {
                        await sw2.WriteLineAsync("Id,EventType,ActorEmail,IpAddress,Message,CreatedAt");
                        foreach (var a in old)
                        {
                            await sw2.WriteLineAsync(string.Join(",",
                                a.Id, Csv(a.EventType), Csv(a.ActorEmail), Csv(a.IpAddress),
                                Csv(a.Message), a.CreatedAt.ToString("o")));
                        }
                    }

                    ctx.AuditLogs.RemoveRange(old);
                    await ctx.SaveChangesAsync();

                    AppLogger.Create<StaleTaskCleanupJob>()?.LogInformation("CleanupJob archived {Count} audit rows -> {Path} in {Ms}ms",
                        old.Length, archivePath, sw.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Create<StaleTaskCleanupJob>()?.LogError(ex, "CleanupJob tick failed");
            }
            finally
            {
                Interlocked.Exchange(ref _running, 0);
            }
        }

        private static string Csv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var needsQuote = s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0;
            return needsQuote ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
        }
    }
}
