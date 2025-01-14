﻿using System.ComponentModel.DataAnnotations;

namespace ServerApp.BLL.ViewModels.Authentication
{
    public class ResetPasswordVm
    {
        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required!")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
