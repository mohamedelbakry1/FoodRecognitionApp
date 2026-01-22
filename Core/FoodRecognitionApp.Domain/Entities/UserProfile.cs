using FoodRecognitionApp.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class UserProfile : BaseEntity<int>
    {
        #region Properties
        public int Age { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public Gender Gender { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public GoalType GoalType { get; set; }
        public decimal Daily_Calories { get; set; }
        #endregion

        #region UserProfile - UserAccount
        public int AccountId { get; set; }
        public UserAccount UserAccount { get; set; } = null!;
        #endregion
    }
}
