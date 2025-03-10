using Domain.Entities.OrderEntities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderEntity=Domain.Entities.OrderEntities.Order;

namespace Persistence.Data.Configurations
{
    internal class OrderConfigurations : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderEntity> builder)
        {
            

            builder.OwnsOne(order => order.ShippingAddress,
                address => address.WithOwner());

            builder.HasMany(order => order.OrderItems)
                .WithOne();

            builder.Property(order => order.PaymentStatus)
                .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<OrderPaymentStatus>(s));

            builder.HasOne(order => order.DeliveryMethod)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);


            builder.Property(t => t.Subtotal)
                    .HasColumnType("decimal(18,3)");
                   
                 
        }
    }
}