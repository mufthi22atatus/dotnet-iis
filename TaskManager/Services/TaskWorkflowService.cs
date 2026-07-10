using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Entities;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;

namespace TaskManager.Services
{
    /// <summary>
    /// Orchestrates multi-step business workflows. Each step calls a separate
    /// SqlQueryService method (separate SqlConnection) producing a distinct MSSQL span.
    /// 
    /// EF6 is used for the initial task CRUD (it also produces MSSQL spans via
    /// System.Data.SqlClient), while SqlQueryService handles the auxiliary inserts
    /// via Microsoft.Data.SqlClient — both appear as MSSQL spans in APM tools.
    /// </summary>
    public class TaskWorkflowService : ITaskWorkflowService
    {
        private readonly Func<AppDbContext> _ctxFactory;
        private readonly SqlQueryService _sql;

        public TaskWorkflowService(Func<AppDbContext> ctxFactory, SqlQueryService sql)
        {
            _ctxFactory = ctxFactory;
            _sql = sql;
        }

        // ================================================================
        // CreateFullTask — 7+ spans
        // ================================================================
        public async Task<DataTable> CreateFullTaskAsync(CreateFullTaskInput input, int actorId)
        {
            int taskId;

            // SPAN 1: INSERT Task via EF
            using (var ctx = _ctxFactory())
            {
                DateTime? dueDate = null;
                if (!string.IsNullOrEmpty(input.DueDate))
                    DateTime.TryParse(input.DueDate, out var dd);

                if (!string.IsNullOrEmpty(input.DueDate) && DateTime.TryParse(input.DueDate, out var parsed))
                    dueDate = parsed;

                var entity = new TaskItem
                {
                    Title = input.Title,
                    Description = input.Description,
                    Priority = (TaskPriority)input.Priority,
                    Status = TaskStatus.Open,
                    DueDate = dueDate,
                    Tag = input.Tag,
                    EstimatedHours = input.EstimatedHours,
                    CreatedById = actorId,
                    AssignedToId = input.AssignedToId,
                    ProjectId = input.ProjectId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                ctx.Tasks.Add(entity);
                await ctx.SaveChangesAsync();
                taskId = entity.Id;
            }

            // SPAN 2: INSERT Assignment (if assigned)
            if (input.AssignedToId.HasValue)
            {
                _sql.InsertTaskAssignment(taskId, input.AssignedToId.Value, actorId);
            }

            // SPAN 3+: INSERT Labels
            if (input.Labels != null)
            {
                foreach (var label in input.Labels)
                {
                    if (!string.IsNullOrWhiteSpace(label))
                        _sql.InsertTaskLabel(taskId, label.Trim(), actorId);
                }
            }

            // SPAN N: INSERT Watchers
            if (input.WatcherIds != null)
            {
                foreach (var watcherId in input.WatcherIds)
                {
                    _sql.InsertTaskWatcher(taskId, watcherId);
                }
            }

            // Always add creator as watcher
            _sql.InsertTaskWatcher(taskId, actorId);

            // SPAN: INSERT TaskHistory — "Created"
            _sql.InsertTaskHistory(taskId, "Created", null, input.Title, actorId);

            // SPAN: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.create", actorId,
                $"Created task '{input.Title}'", "TaskItem", taskId.ToString());

            // SPAN: Read back the created task
            var result = _sql.GetTaskById(taskId);

            AppLogger.Create<TaskWorkflowService>()?.LogInformation("Workflow CreateFullTask completed: TaskId={TaskId}, Actor={Actor}", taskId, actorId);
            return result;
        }

        // ================================================================
        // CloseTask — 5 spans
        // ================================================================
        public async Task<DataTable> CloseTaskAsync(int taskId, int actorId)
        {
            int oldStatus;

            // SPAN 1: UPDATE Task via EF
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;

                oldStatus = (int)entity.Status;
                entity.Status = TaskStatus.Done;
                entity.CompletedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 2: INSERT TaskStatusHistory
            _sql.InsertTaskStatusHistory(taskId, oldStatus, (int)TaskStatus.Done, actorId);

            // SPAN 3: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "Status", ((TaskStatus)oldStatus).ToString(), "Done", actorId);

            // SPAN 4: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.close", actorId,
                $"Closed task {taskId}", "TaskItem", taskId.ToString());

            // SPAN 5: Read updated task
            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // ReopenTask — 5 spans
        // ================================================================
        public async Task<DataTable> ReopenTaskAsync(int taskId, int actorId)
        {
            int oldStatus;

            // SPAN 1: UPDATE Task via EF
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;

                oldStatus = (int)entity.Status;
                entity.Status = TaskStatus.Open;
                entity.CompletedAt = null;
                entity.UpdatedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 2: INSERT TaskStatusHistory
            _sql.InsertTaskStatusHistory(taskId, oldStatus, (int)TaskStatus.Open, actorId);

            // SPAN 3: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "Status", ((TaskStatus)oldStatus).ToString(), "Open", actorId);

            // SPAN 4: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.reopen", actorId,
                $"Reopened task {taskId}", "TaskItem", taskId.ToString());

            // SPAN 5: Read updated task
            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // AssignTask — 5 spans
        // ================================================================
        public async Task<DataTable> AssignTaskAsync(int taskId, int assigneeId, int actorId)
        {
            // SPAN 1: Deactivate previous assignments
            _sql.DeactivatePreviousAssignments(taskId);

            // SPAN 2: INSERT new assignment
            _sql.InsertTaskAssignment(taskId, assigneeId, actorId);

            // SPAN 3: UPDATE Task.AssignedToId via EF
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;
                entity.AssignedToId = assigneeId;
                entity.UpdatedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 4: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "AssignedTo", null, assigneeId.ToString(), actorId);

            // SPAN 5: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.assign", actorId,
                $"Assigned task {taskId} to employee {assigneeId}", "TaskItem", taskId.ToString());

            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // ReassignTask — 6 spans
        // ================================================================
        public async Task<DataTable> ReassignTaskAsync(int taskId, int newAssigneeId, int actorId)
        {
            int? oldAssigneeId = null;

            // SPAN 1: Get current assignee + deactivate
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;
                oldAssigneeId = entity.AssignedToId;
                entity.AssignedToId = newAssigneeId;
                entity.UpdatedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 2: Deactivate old assignments
            _sql.DeactivatePreviousAssignments(taskId);

            // SPAN 3: INSERT new assignment
            _sql.InsertTaskAssignment(taskId, newAssigneeId, actorId);

            // SPAN 4: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "AssignedTo",
                oldAssigneeId?.ToString(), newAssigneeId.ToString(), actorId);

            // SPAN 5: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.reassign", actorId,
                $"Reassigned task {taskId}: {oldAssigneeId} -> {newAssigneeId}", "TaskItem", taskId.ToString());

            // SPAN 6: Notify new assignee
            _sql.InsertNotification(newAssigneeId, "task.assigned", "Task Assigned",
                $"You have been assigned to task {taskId}", "TaskItem", taskId.ToString());

            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // ChangePriority — 4 spans
        // ================================================================
        public async Task<DataTable> ChangePriorityAsync(int taskId, int newPriority, int actorId)
        {
            int oldPriority;

            // SPAN 1: UPDATE via EF
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;
                oldPriority = (int)entity.Priority;
                entity.Priority = (TaskPriority)newPriority;
                entity.UpdatedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 2: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "Priority",
                ((TaskPriority)oldPriority).ToString(), ((TaskPriority)newPriority).ToString(), actorId);

            // SPAN 3: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.priority", actorId,
                $"Priority changed: {(TaskPriority)oldPriority} -> {(TaskPriority)newPriority}",
                "TaskItem", taskId.ToString());

            // SPAN 4: Read back
            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // ChangeStatus — 5 spans
        // ================================================================
        public async Task<DataTable> ChangeStatusAsync(int taskId, int newStatus, int actorId)
        {
            int oldStatus;

            // SPAN 1: UPDATE via EF
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;
                oldStatus = (int)entity.Status;
                entity.Status = (TaskStatus)newStatus;
                entity.UpdatedAt = DateTime.UtcNow;
                if ((TaskStatus)newStatus == TaskStatus.Done)
                    entity.CompletedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 2: INSERT TaskStatusHistory
            _sql.InsertTaskStatusHistory(taskId, oldStatus, newStatus, actorId);

            // SPAN 3: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "Status",
                ((TaskStatus)oldStatus).ToString(), ((TaskStatus)newStatus).ToString(), actorId);

            // SPAN 4: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.status", actorId,
                $"Status changed: {(TaskStatus)oldStatus} -> {(TaskStatus)newStatus}",
                "TaskItem", taskId.ToString());

            // SPAN 5: Read back
            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // ChangeDueDate — 4 spans
        // ================================================================
        public async Task<DataTable> ChangeDueDateAsync(int taskId, string newDueDate, int actorId)
        {
            string oldDueDate;

            // SPAN 1: UPDATE via EF
            using (var ctx = _ctxFactory())
            {
                var entity = await ctx.Tasks.FindAsync(taskId);
                if (entity == null) return null;
                oldDueDate = entity.DueDate?.ToString("yyyy-MM-dd");
                DateTime.TryParse(newDueDate, out var parsed);
                entity.DueDate = parsed;
                entity.UpdatedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();
            }

            // SPAN 2: INSERT TaskHistory
            _sql.InsertTaskHistory(taskId, "DueDate", oldDueDate, newDueDate, actorId);

            // SPAN 3: INSERT AuditLog
            _sql.InsertAuditLogDirect("task.duedate", actorId,
                $"Due date changed: {oldDueDate} -> {newDueDate}", "TaskItem", taskId.ToString());

            // SPAN 4: Read back
            return _sql.GetTaskById(taskId);
        }

        // ================================================================
        // BulkUpdate — 5-15 spans (2 per task + 1 audit)
        // ================================================================
        public async Task<List<DataTable>> BulkUpdateTasksAsync(BulkUpdateInput input, int actorId)
        {
            var results = new List<DataTable>();
            if (input?.TaskIds == null || input.TaskIds.Length == 0) return results;

            foreach (var taskId in input.TaskIds)
            {
                // SPAN: UPDATE via EF
                using (var ctx = _ctxFactory())
                {
                    var entity = await ctx.Tasks.FindAsync(taskId);
                    if (entity == null) continue;

                    if (input.Status.HasValue)
                    {
                        var old = (int)entity.Status;
                        entity.Status = (TaskStatus)input.Status.Value;
                        _sql.InsertTaskHistory(taskId, "Status",
                            ((TaskStatus)old).ToString(), ((TaskStatus)input.Status.Value).ToString(), actorId);
                    }

                    if (input.Priority.HasValue)
                    {
                        var old = (int)entity.Priority;
                        entity.Priority = (TaskPriority)input.Priority.Value;
                        _sql.InsertTaskHistory(taskId, "Priority",
                            ((TaskPriority)old).ToString(), ((TaskPriority)input.Priority.Value).ToString(), actorId);
                    }

                    if (input.AssignedToId.HasValue)
                    {
                        entity.AssignedToId = input.AssignedToId.Value;
                    }

                    entity.UpdatedAt = DateTime.UtcNow;
                    await ctx.SaveChangesAsync();
                }

                results.Add(_sql.GetTaskById(taskId));
            }

            // Final audit log for the bulk operation
            _sql.InsertAuditLogDirect("task.bulk_update", actorId,
                $"Bulk updated {input.TaskIds.Length} tasks", "TaskItem", null);

            return results;
        }
    }
}
