﻿//using Domain.Entities.DoctorEntites;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Persistence.Data.Configurations
//{
//    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
//    {
//        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
//        {
//            builder.HasKey(ds => ds.Id);

//            builder.Property(ds => ds.DoctorId)
//                .IsRequired();

//            builder.Property(ds => ds.Day)
//                .IsRequired();

//            builder.Property(ds => ds.StartTime)
//                .IsRequired();

//            builder.Property(ds => ds.EndTime)
//                .IsRequired();

//            builder.HasOne(ds => ds.Doctor)
//                .WithMany(d => d.Schedules)
//                .HasForeignKey(ds => ds.DoctorId)
//;
//            builder.HasIndex(ds => new { ds.DoctorId, ds.Day })
//                .IsUnique();

//        }
//    }
//}
