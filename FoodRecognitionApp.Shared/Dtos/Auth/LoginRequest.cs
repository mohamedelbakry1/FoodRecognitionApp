using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
