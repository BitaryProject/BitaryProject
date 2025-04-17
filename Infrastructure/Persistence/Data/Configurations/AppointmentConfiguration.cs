using Core.Domain.Entities;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure.Persistence.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.Id);

            // Basic properties
            builder.Property(a => a.AppointmentDateTime)
                .IsRequired();

            builder.Property(a => a.Duration)
                .IsRequired()
                .HasConversion(
                    v => v.Ticks,
                    v => TimeSpan.FromTicks(v));

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Notes)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.PetProfile)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PetProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Follow-up appointment relationship (self-referencing)
            builder.HasOne(a => a.FollowUpToAppointment)
                .WithMany()
                .HasForeignKey(a => a.FollowUpToAppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}




