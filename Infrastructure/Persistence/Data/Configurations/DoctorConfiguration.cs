using Domain.Entities.DoctorEntites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.Specialty)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.Phone)
                .HasMaxLength(20);

            builder.Property(d => d.Gender)
                .IsRequired()
                .HasConversion<byte>(); 

            builder.Property(d => d.ClinicId)
                .IsRequired();

            builder.HasOne(d => d.Clinic)
                .WithMany(c => c.Doctors)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.User)
                .WithOne(u => u.DoctorProfile)
                .HasForeignKey<Doctor>(d => d.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.MedicalRecords)
                .WithOne(mr => mr.Doctor)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.HasMany(d => d.Schedules)
                .WithOne(ds => ds.Doctor)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
