using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.Property(I => I.ImageUrl).HasColumnType("varchar").HasMaxLength(1000);

            builder.Property(I => I.UploadTime).HasDefaultValueSql("GETDATE()");

            builder.HasOne(I => I.UserAccount)
                   .WithMany(U => U.Images)
                   .HasForeignKey(I => I.UserId);
        }
    }
}
