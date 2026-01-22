using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class MealItem
    {
        #region Properties
        public decimal Item_Calories { get; set; }
        public decimal Quantity { get; set; }
        #endregion

        #region MealItem - Meal
        public int MealId { get; set; }
        public Meal Meal { get; set; } = null!;
        #endregion

        #region MealItem - Food
        public int FoodId { get; set; }
        public Food Food { get; set; } = null!;
        #endregion
    }
}
