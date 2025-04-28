using Domain.Entities.ProductEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Persistence.Data.Configurations
{
    public class WishListItemConfiguration : IEntityTypeConfiguration<WishListItem>
    {
        public void Configure(EntityTypeBuilder<WishListItem> builder)
        {
            builder.ToTable("WishListItems");
            
            builder.HasKey(wi => wi.Id);
            
            builder.Property(wi => wi.AddedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
                
            // Configure relationship with WishList
            builder.HasOne(wi => wi.WishList)
                .WithMany(w => w.WishListItems)
                .HasForeignKey(wi => wi.WishListId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Configure relationship with Product
            builder.HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 