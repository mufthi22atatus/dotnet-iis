using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskWatchers")]
    public class TaskWatcher
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        public DateTime WatchingSince { get; set; } = DateTime.UtcNow;
    }
}
