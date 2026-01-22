using FoodRecognitionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.Property(U => U.Weight).HasPrecision(5, 2);
            builder.Property(U => U.Height).HasPrecision(5, 2);

            builder.Property(U => U.Daily_Calories).HasPrecision(6, 2);

            builder.HasOne(P => P.UserAccount)
                   .WithOne()
                   .HasForeignKey<UserProfile>(P => P.AccountId);
        }
    }
}
