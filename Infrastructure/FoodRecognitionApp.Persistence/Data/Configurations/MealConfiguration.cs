using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Identity.Entities;
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
            builder.Property(o => o.MealType).HasConversion<string>().HasMaxLength(10);

            builder.HasOne<UserAccount>()
                   .WithMany(U => U.Meals)
                   .HasForeignKey(M => M.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
