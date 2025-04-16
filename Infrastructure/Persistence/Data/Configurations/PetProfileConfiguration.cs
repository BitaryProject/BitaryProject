using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PetProfileConfiguration : IEntityTypeConfiguration<PetProfile>
    {
        public void Configure(EntityTypeBuilder<PetProfile> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.PetName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(p => p.Species)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(p => p.Breed)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.HasMany(p => p.Appointments)
                .WithOne(a => a.PetProfile)
                .HasForeignKey(a => a.PetProfileId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasMany(p => p.MedicalRecords)
                .WithOne(m => m.PetProfile)
                .HasForeignKey(m => m.PetProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 