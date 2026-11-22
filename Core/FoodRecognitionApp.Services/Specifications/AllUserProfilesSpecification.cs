using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class AllUserProfilesSpecification : BaseSpecification<int, UserProfile>
    {
        public AllUserProfilesSpecification() : base(p => true)
        {

        }
    }
}
