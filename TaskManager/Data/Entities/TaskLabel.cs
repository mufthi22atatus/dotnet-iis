using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskLabels")]
    public class TaskLabel
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        [Required, MaxLength(80)]
        public string Label { get; set; }

        public int AddedById { get; set; }

        [ForeignKey(nameof(AddedById))]
        public virtual Employee AddedBy { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
