using Domain.Entities.DoctorEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Persistence.Data.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.ToTable("DoctorSchedules");
            builder.HasKey(ds => ds.Id);

            builder.Property(ds => ds.DoctorId)
                .IsRequired();

            builder.Property(ds => ds.ScheduleDate)
                .IsRequired();

            builder.Property(ds => ds.StartTime)
                .IsRequired();

            builder.Property(ds => ds.EndTime)
                .IsRequired();

            builder.HasOne(ds => ds.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Create index on DoctorId for faster lookups
            builder.HasIndex(ds => ds.DoctorId);
            
            // Create index on ScheduleDate for date-based lookups
            builder.HasIndex(ds => ds.ScheduleDate);
        }
    }
}
