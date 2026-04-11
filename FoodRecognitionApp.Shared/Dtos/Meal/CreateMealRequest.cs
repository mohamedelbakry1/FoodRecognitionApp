using FoodRecognitionApp.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class CreateMealRequest
    {
        [Required(ErrorMessage = "Meal type is required")]
        [RegularExpression("^(Breakfast|Lunch|Dinner|Snack)$",
        ErrorMessage = "Meal type must be Breakfast, Lunch, Dinner, or Snack")]
        public MealType MealType { get; set; }
        [Required(ErrorMessage = "Meal items are required")]
        [MinLength(1, ErrorMessage = "Meal must contain at least one item")]
        public List<MealItemRequest> Items { get; set; } = null!;
    }
}
