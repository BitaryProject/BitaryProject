using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder.Property(a => a.AppointmentDateTime)
                .IsRequired();
                
            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>();
            
            // Many-to-one relationship with PetProfile
            builder.HasOne(a => a.PetProfile)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PetProfileId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Many-to-one relationship with Clinic
            builder.HasOne(a => a.Clinic)
                .WithMany()
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Many-to-one relationship with Doctor
            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}