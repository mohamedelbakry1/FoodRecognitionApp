using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Auth;
using FoodRecognitionApp.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services
{
    public class ServiceManager
        (
            UserManager<UserAccount> _userManager,
            IOptions<JwtOptions> options
        ) : IServiceManager
    {
        public IAuthService AuthService { get; } = new AuthService(_userManager, options);
    }
}
