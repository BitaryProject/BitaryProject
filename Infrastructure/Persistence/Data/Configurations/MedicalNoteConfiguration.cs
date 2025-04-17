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
    public class MedicalNoteConfiguration : IEntityTypeConfiguration<MedicalNote>
    {
        public void Configure(EntityTypeBuilder<MedicalNote> builder)
        {
            builder.HasKey(n => n.Id);
            
            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(2000);
                
            builder.Property(n => n.CreatedAt)
                .IsRequired();
                
            // Many-to-one relationship with MedicalRecord
            builder.HasOne(n => n.MedicalRecord)
                .WithMany(m => m.MedicalNotes)
                .HasForeignKey(n => n.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Many-to-one relationship with Doctor
            builder.HasOne(n => n.Doctor)
                .WithMany()
                .HasForeignKey(n => n.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 




