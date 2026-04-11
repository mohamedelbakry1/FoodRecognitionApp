using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class FoodsByIdSpecification : BaseSpecification<int, Food>
    {
        public FoodsByIdSpecification(List<int> foodsId) : base(F => foodsId.Contains(F.Id))
        {
            Includes.Add(F => F.CategoryFood);
        }
    }
}
