using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("TaskAttachments")]
    public class TaskAttachment
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem TaskItem { get; set; }

        [Required, MaxLength(260)]
        public string FileName { get; set; }

        [Required, MaxLength(120)]
        public string ContentType { get; set; }

        [Required, MaxLength(512)]
        public string StoredPath { get; set; }

        public long SizeBytes { get; set; }

        public int UploadedById { get; set; }

        [ForeignKey(nameof(UploadedById))]
        public virtual Employee UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
