using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class MealItemRequest
    {
        [Required(ErrorMessage = "FoodId is required")]
        public int FoodId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1,5000, ErrorMessage = "Quantity must be between 1 and 5000 grams")]
        public int Quantity { get; set; }
    }
}
