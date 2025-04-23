using Domain.Entities.MedicalRecordEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");
            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.Diagnosis)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(mr => mr.Treatment)
                .HasMaxLength(500);

            builder.Property(mr => mr.RecordDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(mr => mr.Notes)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(mr => mr.Pet)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mr => mr.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mr => mr.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(mr => mr.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}