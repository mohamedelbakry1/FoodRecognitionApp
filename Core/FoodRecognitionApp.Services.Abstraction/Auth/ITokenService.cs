using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Auth
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email, string userName, IReadOnlyList<string> roles);
    }
}
