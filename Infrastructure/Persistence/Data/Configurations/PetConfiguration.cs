//using Domain.Entities.PetEntities;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.EntityFrameworkCore;

//namespace Persistence.Data.Configurations
//{
//    public class PetConfiguration : IEntityTypeConfiguration<Pet>
//    {
//        public void Configure(EntityTypeBuilder<Pet> builder)
//        {
//            builder.ToTable("Pets");
//            builder.HasKey(p => p.Id);

//            builder.Property(p => p.PetName)
//                .IsRequired()
//                .HasMaxLength(100);

//            builder.Property(p => p.UserId)
//                .IsRequired()
//                .HasMaxLength(50);

//            builder.Property(p => p.BirthDate)
//                .IsRequired();

//            builder.Property(p => p.Gender)
//                .IsRequired()
//                .HasConversion<byte>();

//            builder.Property(p => p.PetType)
//                .IsRequired()
//                .HasConversion<byte>();

//            builder.Property(p => p.Color)
//                .HasMaxLength(50);

//            builder.Property(p => p.Avatar)
//                .HasMaxLength(255);

//            builder.HasMany(p => p.MedicalRecords)
//                .WithOne(mr => mr.Pet)
//                .HasForeignKey(mr => mr.PetId)
//                .OnDelete(DeleteBehavior.Restrict);
//        }
//    }
//}