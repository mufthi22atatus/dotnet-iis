using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskStatusHistory")]
    public class TaskStatusHistory
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        public int OldStatus { get; set; }
        public int NewStatus { get; set; }

        public int ChangedById { get; set; }

        [ForeignKey(nameof(ChangedById))]
        public virtual Employee ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
