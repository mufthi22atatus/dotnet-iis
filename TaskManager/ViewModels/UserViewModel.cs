using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class UserEditInput
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string FullName { get; set; }

        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; }

        [MaxLength(80)]
        public string Department { get; set; }

        [Required, MaxLength(32)]
        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
}
