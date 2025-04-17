using Core.Domain.Entities;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations
{
    public class DoctorRatingConfiguration : IEntityTypeConfiguration<DoctorRating>
    {
        public void Configure(EntityTypeBuilder<DoctorRating> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Rating)
                .IsRequired();
                
            builder.Property(x => x.Comment)
                .HasMaxLength(500);
                
            builder.Property(x => x.CreatedDate)
                .IsRequired();
                
            builder.HasOne(x => x.Doctor)
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.PetOwner)
                .WithMany()
                .HasForeignKey(x => x.PetOwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 




