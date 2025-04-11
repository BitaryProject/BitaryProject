using Domain.Entities.PetEntities;
using Domain.Entities.MedicalRecordEntites;
using Domain.Entities.DoctorEntites;
using Domain.Entities.ClinicEntities;
using Domain.Entities.AppointmentEntities;
using Microsoft.EntityFrameworkCore;
using Shared.ClinicModels;
using Persistence.Data.Configurations;

namespace Persistence.Data
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
        //public DbSet<ClinicSearchCriteria> clinicSearchCriterias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.ApplyConfiguration(new PetConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorScheduleConfiguration());
            modelBuilder.Entity<ClinicSearchCriteria>().HasNoKey();
            //modelBuilder.Entity<ClinicAddress>().HasNoKey();

            /*
                        modelBuilder.Entity<Clinic>(builder =>
                        {
                            builder.OwnsOne(c => c.Address); // Configure Address as an owned entity of Clinic
                        });
            */
        }
    }
}
