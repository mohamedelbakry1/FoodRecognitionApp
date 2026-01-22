using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class Food : BaseEntity<int>
    {
        #region Properties
        public string Name { get; set; } = null!;
        public decimal Calories { get; set; }
        public decimal Carbs { get; set; }
        public decimal Protein { get; set; }
        public decimal Fats { get; set; }
        #endregion

        #region Food - Category
        public int CategoryId { get; set; }
        public Category CategoryFood { get; set; } = null!;
        #endregion

        #region Food - RecognitionResult
        public ICollection<RecognitionResult> RecognitionResults { get; set; } = null!;
        #endregion

        #region Food - MealItem
        public ICollection<MealItem> MealItems { get; set; } = null!;
        #endregion
    }
}
