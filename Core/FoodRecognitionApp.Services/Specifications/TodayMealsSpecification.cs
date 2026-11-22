using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class TodayMealsSpecification : BaseSpecification<int, Meal>
    {
        public TodayMealsSpecification(Guid userId) : base(M => M.UserId == userId && M.MealTime.Date == DateTime.UtcNow.Date)
        {
            Includes.Add(M => M.MealItems);
            AddOrderBy(M => M.MealTime);
        }
    }
}
