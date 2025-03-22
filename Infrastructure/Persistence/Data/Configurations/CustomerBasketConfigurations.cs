using Domain.Entities.BasketEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class CustomerBasketConfigurations : IEntityTypeConfiguration<CustomerBasket>
    {
        public void Configure(EntityTypeBuilder<CustomerBasket> builder)
        {
           
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                   .IsRequired();

            
            builder.HasMany(b => b.BasketItems)
                   .WithOne() 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.ShippingPrice)
           .HasColumnType("decimal(18,2)");



        }
    }
}

