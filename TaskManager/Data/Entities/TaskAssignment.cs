using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskAssignments")]
    public class TaskAssignment
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        public int AssignedToId { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public virtual Employee AssignedTo { get; set; }

        public int AssignedById { get; set; }

        [ForeignKey(nameof(AssignedById))]
        public virtual Employee AssignedBy { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UnassignedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
