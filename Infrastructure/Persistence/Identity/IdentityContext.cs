global using UserAddress = Domain.Entities.Identity.Address;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Identity
{
    public class IdentityContext : IdentityDbContext<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<UserAddress>().ToTable("Addresses");
            
            // Configure the User entity with TPH inheritance
            builder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User");
                
            // Configure the UserRole property
            builder.Entity<User>()
                .Property(u => u.UserRole)
                .HasColumnName("UserRole")
                .HasDefaultValue(Domain.Entities.SecurityEntities.Role.PetOwner)
                .HasConversion<byte>();
        }

       public DbSet<UserOTP?> UserOTPs { get; set; }
    }
}
