using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data.Entities
{
    [Table("Employees")]
    public class Employee
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string FullName { get; set; }

        [Required, MaxLength(160)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Required, MaxLength(256)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(64)]
        public string PasswordSalt { get; set; }

        [Required, MaxLength(32)]
        public string Role { get; set; } = "Employee"; // Employee | Manager | Admin

        [MaxLength(80)]
        public string Department { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public int FailedLoginCount { get; set; }

        public virtual ICollection<TaskItem> CreatedTasks { get; set; } = new HashSet<TaskItem>();
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new HashSet<TaskItem>();
    }
}
