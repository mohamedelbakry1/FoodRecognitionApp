using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class UserAccount : IdentityUser<int>
    {
        #region UserAccount - Image
        public ICollection<Image>? Images { get; set; }
        #endregion

        #region UserAccount - Meal
        public ICollection<Meal>? Meals { get; set; }
        #endregion
    }
}
