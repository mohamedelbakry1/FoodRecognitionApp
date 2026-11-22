using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Services.Abstraction.Meals;
using FoodRecognitionApp.Services.Abstraction.Profile;
using FoodRecognitionApp.Services.Abstraction.Reports;
using FoodRecognitionApp.Services.Auth;
using FoodRecognitionApp.Services.FoodRecognition;
using FoodRecognitionApp.Services.Meals;
using FoodRecognitionApp.Services.Profile;
using FoodRecognitionApp.Services.Reports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFoodRecognitionService, FoodRecognitionService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IMealService, MealService>();
            services.AddScoped<IReportService, ReportService>();

            return services;
        }
    }
}
