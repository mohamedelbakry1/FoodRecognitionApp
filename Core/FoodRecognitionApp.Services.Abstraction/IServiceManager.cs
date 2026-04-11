using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Services.Abstraction.Meals;
using FoodRecognitionApp.Services.Abstraction.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
        IProfileService ProfileService { get; }
        IFoodRecognitionService FoodRecognitionService { get; }
        IMealService MealService { get; }
    }
}
