using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(C => C.CategoryName).HasColumnType("varchar").HasMaxLength(50);

            builder.HasMany(C => C.Foods)
                   .WithOne(F => F.CategoryFood)
                   .HasForeignKey(F => F.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
