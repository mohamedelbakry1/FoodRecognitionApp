using FoodRecognitionApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Identity.Entities
{
    public class UserAccount : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = default!;
        #region UserAccount - Image
        public ICollection<Image>? Images { get; set; }
        #endregion

        #region UserAccount - Meal
        public ICollection<Meal>? Meals { get; set; }
        #endregion
    }
}
