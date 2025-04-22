using Domain.Entities.ClinicEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Persistence.Data.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.ToTable("Clinics");
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.ClinicName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<byte>()
                .HasDefaultValue(ClinicStatus.Pending);
                
            builder.Property(c => c.Rating)
                .HasDefaultValue(0);
                
            builder.Property(c => c.OwnerId)
                .IsRequired()
                .HasMaxLength(450);
           /*     
            // Configure relationship with Owner (User)
            builder.HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
           */
                
            // Configure relationship with Doctors
            builder.HasMany(c => c.Doctors)
                .WithOne(d => d.Clinic)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
              
            // Configure relationship with Appointments
            builder.HasMany(c => c.Appointments)
                .WithOne(a => a.Clinic)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
          

            // Configure the owned Address entity
            builder.OwnsOne(c => c.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Name).HasMaxLength(100);
                addressBuilder.Property(a => a.Street).HasMaxLength(100).IsRequired();
                addressBuilder.Property(a => a.City).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.Country).HasMaxLength(50).IsRequired();
            });
        }
    }
}