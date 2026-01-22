using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class RecognitionResultConfiguration : IEntityTypeConfiguration<RecognitionResult>
    {
        public void Configure(EntityTypeBuilder<RecognitionResult> builder)
        {
            builder.HasKey(R => new {R.ImageId,R.FoodId });

            builder.HasOne(R => R.Image)
                   .WithMany(I => I.RecognitionResults)
                   .HasForeignKey(R => R.ImageId);

            builder.HasOne(R => R.Food)
                   .WithMany(F => F.RecognitionResults)
                   .HasForeignKey(R => R.FoodId);
        }
    }
}
