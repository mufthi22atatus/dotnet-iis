using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskDependencies")]
    public class TaskDependency
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        public int DependsOnTaskId { get; set; }

        [ForeignKey(nameof(DependsOnTaskId))]
        public virtual TaskItem DependsOnTask { get; set; }

        [Required, MaxLength(32)]
        public string DependencyType { get; set; } = "BlockedBy";

        public int CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public virtual Employee CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
