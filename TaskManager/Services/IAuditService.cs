using System.Threading.Tasks;

namespace TaskManager.Services
{
    public interface IAuditService
    {
        Task RecordAsync(string eventType, int? actorId, string actorEmail, string ip, string message,
            string entityName = null, string entityId = null, object payload = null);
    }
}
