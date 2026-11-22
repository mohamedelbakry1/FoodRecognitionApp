using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class MealWithItemsByIdAndUserSpecification : BaseSpecification<int , Meal>
    {
        public MealWithItemsByIdAndUserSpecification(Guid userId, int mealId) : base(M => M.UserId == userId && M.Id == mealId)
        {
            Includes.Add(M => M.MealItems);
        }
    }
}
