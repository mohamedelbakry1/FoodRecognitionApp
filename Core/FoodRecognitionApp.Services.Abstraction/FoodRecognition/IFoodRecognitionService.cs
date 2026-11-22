using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.FoodRecognition
{
    public interface IFoodRecognitionService
    {
        Task<Result<FoodRecognitionResponse?>> RecognizeFoodAsync(Guid userId,FoodRecognitionRequest request, CancellationToken ct = default);
        Task<Result<ICollection<RecentRecognitionResponse>>> GetRecentRecognitionsAsync(Guid userId, CancellationToken ct = default);
    }
}
