using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    public enum TaskStatus
    {
        Open = 0,
        InProgress = 1,
        Blocked = 2,
        InReview = 3,
        Done = 4,
        Cancelled = 5
    }

    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    [Table("Tasks")]
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Open;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public int CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public virtual Employee CreatedBy { get; set; }

        public int? AssignedToId { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public virtual Employee AssignedTo { get; set; }

        public int? ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        [MaxLength(80)]
        public string Tag { get; set; }

        public int EstimatedHours { get; set; }
        public int LoggedHours { get; set; }

        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new HashSet<TaskAttachment>();
        public virtual ICollection<TaskComment> Comments { get; set; } = new HashSet<TaskComment>();
        public virtual ICollection<TaskHistory> History { get; set; } = new HashSet<TaskHistory>();
        public virtual ICollection<TaskLabel> Labels { get; set; } = new HashSet<TaskLabel>();
        public virtual ICollection<TaskWatcher> Watchers { get; set; } = new HashSet<TaskWatcher>();
        public virtual ICollection<TaskAssignment> Assignments { get; set; } = new HashSet<TaskAssignment>();
        public virtual ICollection<TimeLog> TimeLogs { get; set; } = new HashSet<TimeLog>();
    }
}
