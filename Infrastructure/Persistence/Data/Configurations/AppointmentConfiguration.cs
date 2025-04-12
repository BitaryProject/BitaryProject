/*using Domain.Entities.AppointmentEntities;
using Domain.Entities.MedicalRecordEntites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.PetId)
                .IsRequired();

            builder.Property(a => a.ClinicId)
                .IsRequired();

            builder.Property(a => a.AppointmentDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Notes)
                .HasMaxLength(500);

            builder.HasOne(a => a.PetProfile)
                .WithMany() 
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة الـ Many-to-One مع Clinic
            builder.HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Doctor)
                .WithMany() 
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.MedicalRecord)
                .WithOne(mr => mr.Appointment)
                .HasForeignKey<MedicalRecord>(mr => mr.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
*/