//using Domain.Entities.DoctorEntites;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.EntityFrameworkCore;

//namespace Persistence.Data.Configurations
//{
//    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
//    {
//        public void Configure(EntityTypeBuilder<Doctor> builder)
//        {
//            builder.ToTable("Doctors");
//            builder.HasKey(d => d.Id);

//            // Property configurations
//            builder.Property(d => d.Name)
//                .IsRequired()
//                .HasMaxLength(150);

//            builder.Property(d => d.Specialty)
//                .IsRequired()
//                .HasMaxLength(100);

//            builder.Property(d => d.Email)
//                .IsRequired()
//                .HasMaxLength(255);

//            builder.Property(d => d.Phone)
//                .HasMaxLength(20);

//            builder.Property(d => d.Gender)
//                .IsRequired()
//                .HasConversion<byte>();

//            // Critical Fix: Add Clinic relationship
//            builder.HasOne(d => d.Clinic)
//                .WithMany(c => c.Doctors) // Ensure Clinic.Doctors is ICollection<Doctor>
//                .HasForeignKey(d => d.ClinicId)
//                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade

//            // User relationship
//            builder.HasOne(d => d.User)
//                .WithOne(u => u.DoctorProfile)
//                .HasForeignKey<Doctor>(d => d.UserId)
//                .IsRequired(false)
//                .OnDelete(DeleteBehavior.Restrict);

//            // MedicalRecords relationship
//            builder.HasMany(d => d.MedicalRecords)
//                .WithOne(mr => mr.Doctor)
//                .HasForeignKey(mr => mr.DoctorId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // Schedules relationship
//            builder.HasMany(d => d.Schedules)
//                .WithOne(ds => ds.Doctor)
//                .HasForeignKey(ds => ds.DoctorId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // Appointments relationship
//            builder.HasMany(d => d.Appointments)
//                .WithOne(a => a.Doctor)
//                .HasForeignKey(a => a.DoctorId)
//                .OnDelete(DeleteBehavior.Restrict);
//        }
//    }
//}