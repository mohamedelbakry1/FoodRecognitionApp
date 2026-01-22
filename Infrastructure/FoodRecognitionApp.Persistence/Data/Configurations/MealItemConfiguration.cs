using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class MealItemConfiguration : IEntityTypeConfiguration<MealItem>
    {
        public void Configure(EntityTypeBuilder<MealItem> builder)
        {
            builder.HasKey(MI => new { MI.MealId, MI.FoodId });

            builder.Property(MI => MI.Item_Calories).HasPrecision(6,2);
            builder.Property(MI => MI.Quantity).HasPrecision(6,2);

            builder.HasOne(MI => MI.Meal)
                   .WithMany(M => M.MealItems)
                   .HasForeignKey(MI => MI.MealId);

            builder.HasOne(MI => MI.Food)
                   .WithMany(M => M.MealItems)
                   .HasForeignKey(MI => MI.FoodId);
        }
    }
}
