/*
using Domain.Entities.PetEntities;
using Domain.Entities.MedicalRecordEntites;
using Domain.Entities.DoctorEntites;
using Domain.Entities.ClinicEntities;
using Domain.Entities.AppointmentEntities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data.Configurations;

namespace Infrastructure.Persistence.Data
{
    public class NewModuleContext : DbContext
    {
        public NewModuleContext(DbContextOptions<NewModuleContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply configurations for entities
            modelBuilder.ApplyConfiguration(new PetConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorScheduleConfiguration());
        }
    }
}

//            //modelBuilder.Entity<ClinicAddress>().HasNoKey();

//            /*
//                        modelBuilder.Entity<Clinic>(builder =>
//                        {
//                            builder.OwnsOne(c => c.Address); // Configure Address as an owned entity of Clinic
//                        });
//            */
