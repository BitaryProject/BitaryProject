using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.RecordDate)
                .IsRequired();
                
            builder.Property(m => m.Diagnosis)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(m => m.Treatment)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(m => m.AdditionalNotes)
                .HasMaxLength(1000);
                
            // Many-to-one relationship with PetProfile
            builder.HasOne(m => m.PetProfile)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PetProfileId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Many-to-one relationship with Doctor
            builder.HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}