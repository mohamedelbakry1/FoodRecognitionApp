using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Services.Abstraction.AIModel;
using FoodRecognitionApp.Services.Abstraction.AttachmentService;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Services.Abstraction.Meals;
using FoodRecognitionApp.Services.Abstraction.Profile;
using FoodRecognitionApp.Services.Auth;
using FoodRecognitionApp.Services.FoodRecognition;
using FoodRecognitionApp.Services.Meals;
using FoodRecognitionApp.Services.Profile;
using FoodRecognitionApp.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services
{
    public class ServiceManager
        (
            IUnitOfWork _unitOfWork,
            IAttachmentService _attachmentService,
            IAIModelService _aIModelService,
            UserManager<UserAccount> _userManager,
            IHttpContextAccessor _httpContextAccessor,
            IOptions<JwtOptions> options,
            IEmailService emailService,
            IMemoryCache memoryCache
        ) : IServiceManager
    {
        public IAuthService AuthService { get; } = new AuthService(_userManager, options,emailService,memoryCache);

        public IProfileService ProfileService { get; } = new ProfileService(_unitOfWork);

        public IFoodRecognitionService FoodRecognitionService { get; } = new FoodRecognitionService(_unitOfWork, _attachmentService,_aIModelService,_httpContextAccessor);

        public IMealService MealService { get; } = new MealService(_unitOfWork);
    }
}
