using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.Property(M => M.MealTime).HasDefaultValueSql("GETDATE()");

            builder.Property(M => M.TotalCalories).HasPrecision(6, 2);

            builder.HasOne(M => M.UserAccount)
                   .WithMany(U => U.Meals)
                   .HasForeignKey(M => M.UserId);
        }
    }
}
