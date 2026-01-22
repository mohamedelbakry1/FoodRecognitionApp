using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class Category : BaseEntity<int>
    {
        #region Properties
        public string CategoryName { get; set; } = null!;
        #endregion

        #region Category - Food
        public ICollection<Food>? Foods { get; set; } 
        #endregion
    }
}
