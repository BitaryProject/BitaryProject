using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PetOwnerConfiguration : IEntityTypeConfiguration<PetOwner>
    {
        public void Configure(EntityTypeBuilder<PetOwner> builder)
        {
            builder.HasKey(o => o.Id);
            
            builder.Property(o => o.FullName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(o => o.Address)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(o => o.UserId)
                .HasMaxLength(450); // AspNetUsers Id length
                
            builder.HasMany(o => o.PetProfiles)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 