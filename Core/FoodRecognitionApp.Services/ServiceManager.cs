using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Services.Abstraction.Profile;
using FoodRecognitionApp.Services.Auth;
using FoodRecognitionApp.Services.FoodRecognition;
using FoodRecognitionApp.Services.Profile;
using FoodRecognitionApp.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services
{
    public class ServiceManager
        (
            IUnitOfWork _unitOfWork,
            UserManager<UserAccount> _userManager,
            IOptions<JwtOptions> options,
            IWebHostEnvironment _webHostEnvironment
        ) : IServiceManager
    {
        public IAuthService AuthService { get; } = new AuthService(_userManager, options);

        public IProfileService ProfileService { get; } = new ProfileService(_unitOfWork);

        public IFoodRecognitionService FoodRecognitionService { get; } = new FoodRecognitionService(_unitOfWork, _webHostEnvironment);
    }
}
