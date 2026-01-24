using FoodRecognitionApp.Services.Abstraction.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
    }
}
