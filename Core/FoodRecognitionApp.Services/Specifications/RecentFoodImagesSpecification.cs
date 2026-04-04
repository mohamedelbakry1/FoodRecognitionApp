using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class RecentFoodImagesSpecification : BaseSpecification<int, Image>
    {
        public RecentFoodImagesSpecification(int UserId) : base(img => img.UserId == UserId && img.RecognitionResults.Any())
        {
            Includes.Add(img => img.RecognitionResults);

            AddOrderByDescending(img => img.UploadTime);

            ApplyPagination(2, 1);
        }
    }
}
