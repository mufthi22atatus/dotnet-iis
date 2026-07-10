using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data.Entities;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;

namespace TaskManager.Data.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<TaskItem> GetWithDetailsAsync(int id);
        Task<TaskItem[]> ListForEmployeeAsync(int employeeId, bool includeDone);
        Task<TaskItem[]> ListAllAsync(int take = 100);
        Task<TaskItem[]> ListStaleAsync(DateTime cutoffUtc);
    }

    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(Func<AppDbContext> ctxFactory) : base(ctxFactory) { }

        public Task<TaskItem> GetWithDetailsAsync(int id)
        {
            return Set
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Attachments.Select(a => a.UploadedBy))
                .Include(t => t.Comments.Select(c => c.Author))
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem[]> ListForEmployeeAsync(int employeeId, bool includeDone)
        {
            var q = Set
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == employeeId || t.CreatedById == employeeId);

            if (!includeDone)
                q = q.Where(t => t.Status != TaskStatus.Done && t.Status != TaskStatus.Cancelled);

            return await q.OrderByDescending(t => t.Priority)
                          .ThenBy(t => t.DueDate)
                          .ToArrayAsync();
        }

        public async Task<TaskItem[]> ListAllAsync(int take = 100)
        {
            return await Set
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.UpdatedAt)
                .Take(take)
                .ToArrayAsync();
        }

        public async Task<TaskItem[]> ListStaleAsync(DateTime cutoffUtc)
        {
            return await Set
                .Where(t => t.Status == TaskStatus.Done && t.CompletedAt != null && t.CompletedAt < cutoffUtc)
                .ToArrayAsync();
        }
    }
}
