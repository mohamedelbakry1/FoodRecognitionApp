using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class FoodConfiguration : IEntityTypeConfiguration<Food>
    {
        public void Configure(EntityTypeBuilder<Food> builder)
        {
            builder.Property(F => F.Name).HasColumnType("varchar").HasMaxLength(50);

            builder.Property(F => F.Calories).HasPrecision(6, 2);
            builder.Property(F => F.Carbs).HasPrecision(5, 2);
            builder.Property(F => F.Protein).HasPrecision(5, 2);
            builder.Property(F => F.Fats).HasPrecision(5, 2);
        }
    }
}
