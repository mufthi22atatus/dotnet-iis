using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class RegisterViewModel
    {
        [Required, MaxLength(120)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; }

        [Required, MinLength(8), MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [MaxLength(80)]
        public string Department { get; set; }
    }
}
