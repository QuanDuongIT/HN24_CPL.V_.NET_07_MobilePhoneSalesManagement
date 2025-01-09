using System.ComponentModel.DataAnnotations;
namespace ServerApp.BLL.Services.ViewModels.Authentication
{
    public class RegisterVm
    {
        [Required(ErrorMessage = "User is required!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
    }
}
