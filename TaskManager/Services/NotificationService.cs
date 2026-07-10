using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Data.Entities;

namespace TaskManager.Services
{
    /// <summary>
    /// Stub notification service — real impl would push email/Slack/Teams.
    /// Logs at Information level so APMs see a span per notification call.
    /// </summary>
    public class NotificationService : INotificationService
    {
        public Task NotifyTaskAssignedAsync(TaskItem task, Employee assignee)
        {
            AppLogger.Create<NotificationService>()?.LogInformation("Notify: task {TaskId} '{Title}' assigned to {Email}",
                task.Id, task.Title, assignee?.Email);
            return Task.CompletedTask;
        }

        public Task NotifyTaskDueSoonAsync(TaskItem task)
        {
            AppLogger.Create<NotificationService>()?.LogInformation("Notify: task {TaskId} '{Title}' due {Due}", task.Id, task.Title, task.DueDate);
            return Task.CompletedTask;
        }
    }
}
