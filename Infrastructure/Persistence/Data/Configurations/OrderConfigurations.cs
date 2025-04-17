using Core.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using OrderEntity = Core.Domain.Entities.OrderEntities.Order;

namespace Infrastructure.Persistence.Data.Configurations
{
    internal class OrderConfigurations : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.OwnsOne(
                order => order.ShippingAddress,
                address => address.WithOwner()
            );

            builder.HasMany(order => order.OrderItems)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(order => order.PaymentStatus)
                   .HasConversion(
                       s => s.ToString(),
                       s => Enum.Parse<OrderPaymentStatus>(s)
                   );

            builder.HasOne(order => order.DeliveryMethod)
                   .WithMany()
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(order => order.Subtotal)
                   .HasColumnType("decimal(18,3)");
        }
    }
}







