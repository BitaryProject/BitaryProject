//using Domain.Entities.ClinicEntities;
//using Domain.Entities.DoctorEntites;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Persistence.Data.Configurations
//{
//    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
//    {
//        public void Configure(EntityTypeBuilder<Clinic> builder)
//        {
//            builder.ToTable("Clinics");
//            builder.HasKey(c => c.Id);

//            builder.Property(c => c.ClinicName)
//                .IsRequired()
//                .HasMaxLength(150);

//            // Owned Address configuration
//            builder.OwnsOne(c => c.Address, addressBuilder =>
//            {
//                addressBuilder.Property(a => a.Name).HasMaxLength(100);
//                addressBuilder.Property(a => a.Street).HasMaxLength(200);
//                addressBuilder.Property(a => a.City).HasMaxLength(100);
//                addressBuilder.Property(a => a.Country).HasMaxLength(100);
//            });

//            // Fixed decimal configuration
//            builder.Property(c => c.ExaminationFee)
//                .HasColumnType("decimal(18,2)");

//            // Correct many-to-many configuration
//            builder.HasMany(c => c.Doctors)
//      .WithOne(d => d.Clinic) // Doctor.Clinic is the navigation property
//      .HasForeignKey(d => d.ClinicId)
//      .OnDelete(DeleteBehavior.Restrict);
//        }
//    }
//}