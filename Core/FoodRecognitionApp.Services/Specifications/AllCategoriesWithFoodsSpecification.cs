using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class AllCategoriesWithFoodsSpecification : BaseSpecification<int, Category>
    {
        public AllCategoriesWithFoodsSpecification() : base(c => true)
        {
            Includes.Add(c => c.Foods!);
            AddOrderBy(c => c.CategoryName);
        }
    }
}
