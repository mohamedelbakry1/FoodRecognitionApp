using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class UpdateMealItemsRequest
    {
        [Required(ErrorMessage = "Meal items are required")]
        [MinLength(1, ErrorMessage = "Meal must contain at least one item")]
        public List<MealItemRequest> Items { get; set; } = null!;
    }
}
