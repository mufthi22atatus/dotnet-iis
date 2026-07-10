using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        public long Id { get; set; }

        [Required, MaxLength(64)]
        public string EventType { get; set; }

        [MaxLength(64)]
        public string EntityName { get; set; }

        [MaxLength(64)]
        public string EntityId { get; set; }

        public int? ActorId { get; set; }

        [MaxLength(160)]
        public string ActorEmail { get; set; }

        [MaxLength(64)]
        public string IpAddress { get; set; }

        [MaxLength(512)]
        public string Message { get; set; }

        [MaxLength(4000)]
        public string PayloadJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
