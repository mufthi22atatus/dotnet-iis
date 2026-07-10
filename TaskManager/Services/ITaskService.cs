using System.Threading.Tasks;
using TaskManager.Data.Entities;
using TaskManager.ViewModels;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;

namespace TaskManager.Services
{
    public interface ITaskService
    {
        Task<TaskItem[]> ListForUserAsync(int employeeId, bool includeDone);
        Task<TaskItem[]> ListAllAsync(int take = 100);
        Task<TaskItem> GetAsync(int id);
        Task<TaskItem> CreateAsync(TaskCreateInput input, int actorId);
        Task<TaskItem> UpdateAsync(int id, TaskUpdateInput input, int actorId);
        Task<bool> DeleteAsync(int id, int actorId);
        Task<TaskItem> AssignAsync(int taskId, int assigneeId, int actorId);
        Task<TaskItem> ChangeStatusAsync(int taskId, TaskStatus newStatus, int actorId);
        Task<DashboardViewModel> BuildDashboardAsync();
    }
}
