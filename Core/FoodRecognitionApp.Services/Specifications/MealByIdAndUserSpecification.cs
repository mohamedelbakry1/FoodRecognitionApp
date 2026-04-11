using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class MealByIdAndUserSpecification : BaseSpecification<int , Meal>
    {
        public MealByIdAndUserSpecification(int userId, int mealId) : base(M => M.UserId == userId && M.Id == mealId)  
        {
            
        }
    }
}
