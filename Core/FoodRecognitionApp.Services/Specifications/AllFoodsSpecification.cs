using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class AllFoodsSpecification : BaseSpecification<int,Food>
    {
        public AllFoodsSpecification() : base(F => true)
        {
            Includes.Add(F => F.CategoryFood);
            AddOrderBy(F => F.Name);
        }
    }
}
