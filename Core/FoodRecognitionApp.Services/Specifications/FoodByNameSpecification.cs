using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class FoodByNameSpecification : BaseSpecification<int,Food>
    {
        public FoodByNameSpecification(string foodName) : base(F => F.Name.ToLower().Trim() == foodName.ToLower().Trim().Replace("_", " "))
        {
            Includes.Add(F => F.CategoryFood);
        }
    }
}
