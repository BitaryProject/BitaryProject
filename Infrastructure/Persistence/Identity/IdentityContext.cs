global using UserAddress = Core.Domain.Entities.Identity.Address;
using Core.Domain.Entities.IdentityEntities;
using Core.Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Identity
{
    public class IdentityContext : IdentityDbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserAddress>().ToTable("Addresses");
        }

       public DbSet<UserOTP> UserOTPs { get; set; }
    }
}






