using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class RecentFoodImagesSpecification : BaseSpecification<int, Image>
    {
        public RecentFoodImagesSpecification(Guid UserId) : base(img => img.UserId == UserId && img.RecognitionResults.Any())
        {
            Includes.Add(img => img.RecognitionResults);

            AddOrderByDescending(img => img.UploadTime);

            ApplyPagination(2, 1);
        }
    }
}
