using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "User name must be between 3 and 100 characters")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Password confirmation is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
