using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.AIModel
{
    public interface IAIModelService
    {
        Task<Result<ICollection<AIModelResponse>>> ClassifyFoodAsync(IFormFile image, CancellationToken ct = default);
    }
}
