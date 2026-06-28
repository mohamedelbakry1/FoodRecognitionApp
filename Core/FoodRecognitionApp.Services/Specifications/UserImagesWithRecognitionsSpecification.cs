using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class UserImagesWithRecognitionsSpecification : BaseSpecification<int, Image>
    {
        public UserImagesWithRecognitionsSpecification(int userId) : base(i => i.UserId == userId)
        {
            Includes.Add(i => i.RecognitionResults);
            AddOrderByDescending(i => i.UploadTime);
        }
    }
}
