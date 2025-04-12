/*using Domain.Entities.PetEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PetName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.UserId)
                .IsRequired()
                .HasMaxLength(50); 

            builder.Property(p => p.BirthDate)
                .IsRequired();

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasConversion<byte>(); 

            builder.Property(p => p.PetType)
                .IsRequired()
                .HasConversion<byte>();

            builder.Property(p => p.Color)
                .HasMaxLength(50);

            builder.Property(p => p.Avatar)
                .HasMaxLength(255);

            builder.HasMany(p => p.MedicalRecords)
                .WithOne(mr => mr.PetProfile)
                .HasForeignKey(mr => mr.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
*/