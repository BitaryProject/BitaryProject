using Domain.Entities.ClinicEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Persistence.Data.Configurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Ratings");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.RatingValue)
                .IsRequired()
                .HasDefaultValue(0);
                
            builder.Property(r => r.Comment)
                .HasMaxLength(500);
                
            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(r => r.UserId)
                .IsRequired()
                .HasMaxLength(450);
                
            builder.Property(r => r.ClinicId)
                .IsRequired();
                
            // Configure relationship with Clinic
            builder.HasOne(r => r.Clinic)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 