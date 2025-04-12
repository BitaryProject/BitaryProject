/*using Domain.Entities.ClinicEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClinicName)
                .IsRequired()
                .HasMaxLength(150);




            builder.OwnsOne(c => c.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Name)
                    .HasMaxLength(100);
                addressBuilder.Property(a => a.Street)
                    .HasMaxLength(200);
                addressBuilder.Property(a => a.City)
                    .HasMaxLength(100);
                addressBuilder.Property(a => a.Country)
                    .HasMaxLength(100);
            });

            builder.Property(c => c.ExaminationFee)
                .HasColumnType("decimal(18, 2)"); 

            builder.Property(c => c.Rating)
                .IsRequired()
                .HasColumnType("float");

            builder.HasMany(c => c.Doctors)
                .WithOne(d => d.Clinic)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Appointments)
                .WithOne(a => a.Clinic)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);



        }
    }
}
*/