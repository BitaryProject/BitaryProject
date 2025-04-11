using Domain.Entities.MedicalRecordEntites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.PetId)
                .IsRequired();

            builder.Property(mr => mr.DoctorId)
                .IsRequired();

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

            builder.Property(mr => mr.AppointmentId)
                .IsRequired();

            builder.HasOne(mr => mr.PetProfile)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PetId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(mr => mr.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(mr => mr.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(mr => mr.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
