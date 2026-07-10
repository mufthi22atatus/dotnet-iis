using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskComments")]
    public class TaskComment
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public virtual Employee Author { get; set; }

        [Required, MaxLength(2000)]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
