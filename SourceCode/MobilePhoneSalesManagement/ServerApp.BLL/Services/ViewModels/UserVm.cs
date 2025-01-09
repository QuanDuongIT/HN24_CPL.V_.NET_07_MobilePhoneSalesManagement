using System.ComponentModel.DataAnnotations;

namespace ServerApp.BLL.Services.ViewModels
{
    public class UserVm
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        public bool Status { get; set; } = true;
        public string Role { get; set; } = "client";
        public DateTime LastOnlineAt { get; set; }

        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
