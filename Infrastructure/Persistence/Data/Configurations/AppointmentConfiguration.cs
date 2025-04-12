//using Domain.Entities.AppointmentEntities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Persistence.Data.Configurations
//{
//    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
//    {
//        public void Configure(EntityTypeBuilder<Appointment> builder)
//        {
//            builder.ToTable("Appointments");
//            builder.HasKey(a => a.Id);

//            // Relationships with correct delete behavior
//            builder.HasOne(a => a.Clinic)
//                .WithMany(c => c.Appointments)
//                .HasForeignKey(a => a.ClinicId)
//                .OnDelete(DeleteBehavior.Restrict);

//            builder.HasOne(a => a.Doctor)
//                .WithMany(d => d.Appointments)
//                .HasForeignKey(a => a.DoctorId)
//                .OnDelete(DeleteBehavior.Restrict);
//        }
//    }
//}