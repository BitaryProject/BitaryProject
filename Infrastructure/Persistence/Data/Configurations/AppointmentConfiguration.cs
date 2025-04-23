using Domain.Entities.AppointmentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(450); // Standard length for Identity user IDs

            builder.Property(a => a.AppointmentDate)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<byte>();
                
            builder.Property(a => a.Notes)
                .HasMaxLength(500);
                
            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(a => a.PetProfile)
                .WithMany()
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(a => a.MedicalRecord)
                .WithOne()
                .HasForeignKey<Appointment>(a => a.MedicalRecordId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}