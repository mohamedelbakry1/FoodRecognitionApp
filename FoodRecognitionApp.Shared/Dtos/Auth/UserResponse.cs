using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Auth
{
    public class UserResponse
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
