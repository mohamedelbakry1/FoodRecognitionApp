using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Auth
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage ="Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "OTP is Required")]
        public string Otp { get; set; } = null!;
    }
}
