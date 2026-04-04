using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.FoodRecognition
{
    public interface IFoodRecognitionService
    {
        Task<FoodRecognitionResponse?> RecognizeFoodAsync(int userId,FoodRecognitionRequest request);
        Task<IEnumerable<RecentRecognitionResponse>> GetRecentRecognitionsAsync(int userId);
    }
}
