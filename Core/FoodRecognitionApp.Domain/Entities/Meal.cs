using FoodRecognitionApp.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class Meal : BaseEntity<int>
    {
        #region Properties
        public MealType MealType { get; set; }
        public DateTime MealTime { get; set; }
        public decimal TotalCalories { get; set; }
        #endregion

        #region Meal - UserAccount
        public int UserId { get; set; }
        public UserAccount UserAccount { get; set; } = null!;
        #endregion

        #region Meal - MealItem
        public ICollection<MealItem> MealItems { get; set; } = null!;
        #endregion
    }
}
