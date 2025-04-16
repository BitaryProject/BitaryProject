using Domain.Entities.BasketEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class BasketItemConfigurations : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Quantity)
                   .IsRequired();

            // Configure the owned entity correctly
            builder.OwnsOne(
                b => b.Product,
                product =>
                {
                    product.Property(p => p.ProductId)
                        .HasColumnName("ProductId")
                        .IsRequired();

                    product.Property(p => p.ProductName)
                        .HasColumnName("ProductName")
                        .IsRequired()
                        .HasMaxLength(100);

                    product.Property(p => p.PictureUrl)
                        .HasColumnName("PictureUrl")
                        .HasMaxLength(300);
                }
            );

            // Ensure relationship with CustomerBasket is configured properly
            builder.HasOne<CustomerBasket>()
                .WithMany(cb => cb.BasketItems)
                .HasForeignKey("CustomerBasketId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}