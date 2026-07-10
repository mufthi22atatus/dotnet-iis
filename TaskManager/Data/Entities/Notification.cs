using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("Notifications")]
    public class Notification
    {
        public long Id { get; set; }

        public int RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public virtual Employee Recipient { get; set; }

        [Required, MaxLength(64)]
        public string Type { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Message { get; set; }

        [MaxLength(64)]
        public string RelatedEntityType { get; set; }

        [MaxLength(64)]
        public string RelatedEntityId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }
    }
}
