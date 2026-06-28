using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class AllUserProfilesSpecification : BaseSpecification<int, UserProfile>
    {
        public AllUserProfilesSpecification() : base(p => true)
        {
            Includes.Add(p => p.UserAccount);
        }
    }
}
