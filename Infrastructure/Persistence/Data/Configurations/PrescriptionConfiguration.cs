using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.PrescriptionDate)
                .IsRequired();
                
            builder.Property(p => p.StartDate)
                .IsRequired();
                
            builder.Property(p => p.EndDate)
                .IsRequired();
                
            builder.Property(p => p.Medication)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(p => p.Dosage)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(p => p.Instructions)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(p => p.Notes)
                .HasMaxLength(1000);
                
            // Many-to-one relationship with PetProfile
            builder.HasOne(p => p.PetProfile)
                .WithMany()
                .HasForeignKey(p => p.PetProfileId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Many-to-one relationship with Doctor
            builder.HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Many-to-one relationship with MedicalRecord (optional)
            builder.HasOne(p => p.MedicalRecord)
                .WithMany()
                .HasForeignKey(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
} 