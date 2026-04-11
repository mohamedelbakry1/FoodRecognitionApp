using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class MealsByDateSpecification : BaseSpecification<int , Meal>
    {
        public MealsByDateSpecification(int userId, DateTime date) : base(M => M.UserId == userId && M.MealTime.Date == date)
        {
            Includes.Add(M => M.MealItems);
            AddOrderBy(M => M.MealTime);
        }
    }
}
