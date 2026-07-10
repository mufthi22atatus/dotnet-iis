using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TimeLogs")]
    public class TimeLog
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StoppedAt { get; set; }

        public int DurationMinutes { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
