using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Identity.Entities;
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

            builder.Property(o => o.Gender).HasConversion<string>().HasMaxLength(6);
            builder.Property(o => o.ActivityLevel).HasConversion<string>().HasMaxLength(20);
            builder.Property(o => o.GoalType).HasConversion<string>().HasMaxLength(15);

            builder.HasOne<UserAccount>()
                   .WithOne()
                   .HasForeignKey<UserProfile>(P => P.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("Check_Age_Valid", "Age between 3 and 100");
                Tb.HasCheckConstraint("Check_Height_Valid", "Height between 100 and 250");
                Tb.HasCheckConstraint("Check_Weight_Valid", "Weight between 30 and 300");
            });
        }
    }
}
