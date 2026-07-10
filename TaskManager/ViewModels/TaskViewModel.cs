using System;
using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Entities;

namespace TaskManager.ViewModels
{
    public class TaskCreateInput
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public int? AssignedToId { get; set; }

        [MaxLength(80)]
        public string Tag { get; set; }

        [Range(0, 1000)]
        public int EstimatedHours { get; set; }
    }

    public class TaskUpdateInput
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [MaxLength(80)]
        public string Tag { get; set; }

        [Range(0, 1000)]
        public int EstimatedHours { get; set; }

        [Range(0, 1000)]
        public int LoggedHours { get; set; }
    }

    public class TaskListRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
