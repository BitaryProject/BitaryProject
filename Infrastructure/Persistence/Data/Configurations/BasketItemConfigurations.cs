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
        }
    }
}
