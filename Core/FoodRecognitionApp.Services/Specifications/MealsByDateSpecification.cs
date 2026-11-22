using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class MealsByDateSpecification : BaseSpecification<int , Meal>
    {
        public MealsByDateSpecification(Guid userId, DateTime date) : base(M => M.UserId == userId && M.MealTime.Date == date)
        {
            Includes.Add(M => M.MealItems);
            AddOrderBy(M => M.MealTime);
        }
    }
}
