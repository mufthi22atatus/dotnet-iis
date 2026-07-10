using System.Threading.Tasks;
using TaskManager.Data.Entities;

namespace TaskManager.Services
{
    public interface INotificationService
    {
        Task NotifyTaskAssignedAsync(TaskItem task, Employee assignee);
        Task NotifyTaskDueSoonAsync(TaskItem task);
    }
}
