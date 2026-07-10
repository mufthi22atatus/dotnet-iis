using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TaskManager.Services
{
    /// <summary>
    /// Orchestrates multi-step business workflows where each step produces
    /// a separate MSSQL span via SqlQueryService.
    /// </summary>
    public interface ITaskWorkflowService
    {
        /// <summary>Create task + assignment + labels + watcher + history + audit + read back (7+ spans)</summary>
        Task<DataTable> CreateFullTaskAsync(CreateFullTaskInput input, int actorId);

        /// <summary>Close task: update status + status history + task history + audit + read (5 spans)</summary>
        Task<DataTable> CloseTaskAsync(int taskId, int actorId);

        /// <summary>Reopen task: update status + status history + task history + audit + read (5 spans)</summary>
        Task<DataTable> ReopenTaskAsync(int taskId, int actorId);

        /// <summary>Assign: deactivate old + insert assignment + history + audit + notification (5 spans)</summary>
        Task<DataTable> AssignTaskAsync(int taskId, int assigneeId, int actorId);

        /// <summary>Reassign: deactivate old + insert new + history + audit + notify old + notify new (6 spans)</summary>
        Task<DataTable> ReassignTaskAsync(int taskId, int newAssigneeId, int actorId);

        /// <summary>Change priority + history + audit + read (4 spans)</summary>
        Task<DataTable> ChangePriorityAsync(int taskId, int newPriority, int actorId);

        /// <summary>Change status + status history + task history + audit + read (5 spans)</summary>
        Task<DataTable> ChangeStatusAsync(int taskId, int newStatus, int actorId);

        /// <summary>Change due date + history + audit + read (4 spans)</summary>
        Task<DataTable> ChangeDueDateAsync(int taskId, string newDueDate, int actorId);

        /// <summary>Bulk update multiple tasks (5-15 spans depending on count)</summary>
        Task<List<DataTable>> BulkUpdateTasksAsync(BulkUpdateInput input, int actorId);
    }

    public class CreateFullTaskInput
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } = 1;
        public int? AssignedToId { get; set; }
        public int? ProjectId { get; set; }
        public string DueDate { get; set; }
        public string Tag { get; set; }
        public int EstimatedHours { get; set; }
        public string[] Labels { get; set; }
        public int[] WatcherIds { get; set; }
    }

    public class BulkUpdateInput
    {
        public int[] TaskIds { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? AssignedToId { get; set; }
    }
}
