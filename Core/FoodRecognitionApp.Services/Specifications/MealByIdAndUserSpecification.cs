using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class MealByIdAndUserSpecification : BaseSpecification<int , Meal>
    {
        public MealByIdAndUserSpecification(Guid userId, int mealId) : base(M => M.UserId == userId && M.Id == mealId)  
        {
            
        }
    }
}
