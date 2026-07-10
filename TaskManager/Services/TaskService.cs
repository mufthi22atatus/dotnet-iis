using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Data.Entities;
using TaskManager.Data.Repositories;
using TaskManager.ViewModels;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;

namespace TaskManager.Services
{
    public class TaskService : ITaskService
    {
        private const string DashboardCacheKey = "dashboard:summary";

        private readonly ITaskRepository _tasks;
        private readonly IUserRepository _users;
        private readonly INotificationService _notify;
        private readonly IAuditService _audit;
        private readonly ICacheService _cache;

        public TaskService(ITaskRepository tasks, IUserRepository users,
            INotificationService notify, IAuditService audit, ICacheService cache)
        {
            _tasks = tasks;
            _users = users;
            _notify = notify;
            _audit = audit;
            _cache = cache;
        }

        public async Task<TaskItem[]> ListForUserAsync(int employeeId, bool includeDone)
        {
            using ((IDisposable)_tasks)
                return await _tasks.ListForEmployeeAsync(employeeId, includeDone);
        }

        public async Task<TaskItem[]> ListAllAsync(int take = 100)
        {
            using ((IDisposable)_tasks)
                return await _tasks.ListAllAsync(take);
        }

        public async Task<TaskItem> GetAsync(int id)
        {
            using ((IDisposable)_tasks)
                return await _tasks.GetWithDetailsAsync(id);
        }

        public async Task<TaskItem> CreateAsync(TaskCreateInput input, int actorId)
        {
            using ((IDisposable)_tasks)
            {
                var entity = new TaskItem
                {
                    Title = input.Title,
                    Description = input.Description,
                    Priority = input.Priority,
                    Status = TaskStatus.Open,
                    DueDate = input.DueDate,
                    Tag = input.Tag,
                    EstimatedHours = input.EstimatedHours,
                    CreatedById = actorId,
                    AssignedToId = input.AssignedToId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _tasks.Add(entity);
                await _tasks.SaveChangesAsync();

                _cache.Remove(DashboardCacheKey);
                await _audit.RecordAsync("task.create", actorId, null, null,
                    $"Created task '{entity.Title}'", "TaskItem", entity.Id.ToString());

                if (entity.AssignedToId.HasValue)
                {
                    var assignee = await _users.GetByIdAsync(entity.AssignedToId.Value);
                    if (assignee != null) await _notify.NotifyTaskAssignedAsync(entity, assignee);
                }

                AppLogger.Create<TaskService>()?.LogInformation("Task {Id} created by user {Actor}", entity.Id, actorId);
                return entity;
            }
        }

        public async Task<TaskItem> UpdateAsync(int id, TaskUpdateInput input, int actorId)
        {
            using ((IDisposable)_tasks)
            {
                var entity = await _tasks.GetByIdAsync(id);
                if (entity == null) return null;

                entity.Title = input.Title;
                entity.Description = input.Description;
                entity.Priority = input.Priority;
                entity.Status = input.Status;
                entity.DueDate = input.DueDate;
                entity.Tag = input.Tag;
                entity.EstimatedHours = input.EstimatedHours;
                entity.LoggedHours = input.LoggedHours;
                entity.UpdatedAt = DateTime.UtcNow;
                if (input.Status == TaskStatus.Done && entity.CompletedAt == null)
                    entity.CompletedAt = DateTime.UtcNow;

                _tasks.Update(entity);
                await _tasks.SaveChangesAsync();
                _cache.Remove(DashboardCacheKey);
                await _audit.RecordAsync("task.update", actorId, null, null,
                    $"Updated task '{entity.Title}'", "TaskItem", id.ToString());

                return entity;
            }
        }

        public async Task<bool> DeleteAsync(int id, int actorId)
        {
            using ((IDisposable)_tasks)
            {
                var entity = await _tasks.GetByIdAsync(id);
                if (entity == null) return false;
                _tasks.Remove(entity);
                await _tasks.SaveChangesAsync();
                _cache.Remove(DashboardCacheKey);
                await _audit.RecordAsync("task.delete", actorId, null, null,
                    $"Deleted task {id}", "TaskItem", id.ToString());
                return true;
            }
        }

        public async Task<TaskItem> AssignAsync(int taskId, int assigneeId, int actorId)
        {
            using ((IDisposable)_tasks)
            {
                var entity = await _tasks.GetByIdAsync(taskId);
                if (entity == null) return null;

                entity.AssignedToId = assigneeId;
                entity.UpdatedAt = DateTime.UtcNow;
                _tasks.Update(entity);
                await _tasks.SaveChangesAsync();
                _cache.Remove(DashboardCacheKey);

                var assignee = await _users.GetByIdAsync(assigneeId);
                if (assignee != null) await _notify.NotifyTaskAssignedAsync(entity, assignee);

                await _audit.RecordAsync("task.assign", actorId, null, null,
                    $"Assigned task {taskId} -> {assigneeId}", "TaskItem", taskId.ToString());
                return entity;
            }
        }

        public async Task<TaskItem> ChangeStatusAsync(int taskId, TaskStatus newStatus, int actorId)
        {
            using ((IDisposable)_tasks)
            {
                var entity = await _tasks.GetByIdAsync(taskId);
                if (entity == null) return null;

                entity.Status = newStatus;
                entity.UpdatedAt = DateTime.UtcNow;
                if (newStatus == TaskStatus.Done) entity.CompletedAt = DateTime.UtcNow;

                _tasks.Update(entity);
                await _tasks.SaveChangesAsync();
                _cache.Remove(DashboardCacheKey);

                await _audit.RecordAsync("task.status", actorId, null, null,
                    $"Status -> {newStatus}", "TaskItem", taskId.ToString());
                return entity;
            }
        }

        public async Task<DashboardViewModel> BuildDashboardAsync()
        {
            // Quick cache hit
            var cached = _cache.GetOrAdd<DashboardViewModel>(DashboardCacheKey, TimeSpan.FromSeconds(1), () => null);
            if (cached != null) return cached;

            // Miss → compute async, store with full TTL.
            var ttl = TimeSpan.FromSeconds(int.Parse(
                ConfigurationManager.AppSettings["Cache:DashboardSeconds"] ?? "30"));

            var fresh = await BuildDashboardInner().ConfigureAwait(false);
            _cache.Remove(DashboardCacheKey);
            _cache.GetOrAdd(DashboardCacheKey, ttl, () => fresh);
            return fresh;
        }

        private async Task<DashboardViewModel> BuildDashboardInner()
        {
            using ((IDisposable)_tasks)
            {
                var q = _tasks.Query();
                var total = await q.CountAsync();
                var open = await q.CountAsync(t => t.Status == TaskStatus.Open);
                var inProgress = await q.CountAsync(t => t.Status == TaskStatus.InProgress);
                var done = await q.CountAsync(t => t.Status == TaskStatus.Done);
                var overdue = await q.CountAsync(t => t.DueDate != null
                                                     && t.DueDate < DateTime.UtcNow
                                                     && t.Status != TaskStatus.Done
                                                     && t.Status != TaskStatus.Cancelled);

                var byPriority = await q.GroupBy(t => t.Priority)
                    .Select(g => new DashboardCount { Bucket = g.Key.ToString(), Count = g.Count() })
                    .ToArrayAsync();

                var byStatus = await q.GroupBy(t => t.Status)
                    .Select(g => new DashboardCount { Bucket = g.Key.ToString(), Count = g.Count() })
                    .ToArrayAsync();

                var recent = await q.Include(t => t.AssignedTo)
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(10)
                    .ToArrayAsync();

                return new DashboardViewModel
                {
                    GeneratedAtUtc = DateTime.UtcNow,
                    Total = total,
                    Open = open,
                    InProgress = inProgress,
                    Done = done,
                    Overdue = overdue,
                    ByPriority = byPriority,
                    ByStatus = byStatus,
                    Recent = recent.Select(t => new RecentTaskRow
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status.ToString(),
                        Priority = t.Priority.ToString(),
                        AssignedTo = t.AssignedTo?.FullName,
                        UpdatedAt = t.UpdatedAt
                    }).ToArray()
                };
            }
        }
    }
}
