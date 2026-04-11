using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class TodayMealsSpecification : BaseSpecification<int, Meal>
    {
        public TodayMealsSpecification(int userId) : base(M => M.UserId == userId && M.MealTime.Date == DateTime.Today)
        {
            Includes.Add(M => M.MealItems);
            AddOrderBy(M => M.MealItems);
        }
    }
}
