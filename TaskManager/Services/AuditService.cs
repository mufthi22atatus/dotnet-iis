using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Entities;

namespace TaskManager.Services
{
    public class AuditService : IAuditService
    {
        private readonly Func<AppDbContext> _ctxFactory;

        public AuditService(Func<AppDbContext> ctxFactory)
        {
            _ctxFactory = ctxFactory;
        }

        public async Task RecordAsync(string eventType, int? actorId, string actorEmail, string ip, string message,
            string entityName = null, string entityId = null, object payload = null)
        {
            try
            {
                using (var ctx = _ctxFactory())
                {
                    var entry = new AuditLog
                    {
                        EventType = eventType,
                        ActorId = actorId,
                        ActorEmail = Truncate(actorEmail, 160),
                        IpAddress = Truncate(ip, 64),
                        Message = Truncate(message, 512),
                        EntityName = Truncate(entityName, 64),
                        EntityId = Truncate(entityId, 64),
                        PayloadJson = payload != null ? Truncate(JsonConvert.SerializeObject(payload), 4000) : null,
                        CreatedAt = DateTime.UtcNow
                    };

                    ctx.AuditLogs.Add(entry);
                    await ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Create<AuditService>()?.LogWarning(ex, "Audit write failed for event {EventType}", eventType);
            }
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max);
        }
    }
}
