using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskHistory")]
    public class TaskHistory
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        [Required, MaxLength(64)]
        public string FieldName { get; set; }

        [MaxLength(500)]
        public string OldValue { get; set; }

        [MaxLength(500)]
        public string NewValue { get; set; }

        public int ChangedById { get; set; }

        [ForeignKey(nameof(ChangedById))]
        public virtual Employee ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
