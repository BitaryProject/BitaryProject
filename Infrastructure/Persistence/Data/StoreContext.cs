using Domain.Entities.BasketEntities;
using Domain.Entities.OrderEntities;
using Domain.Entities.PetEntities;
using Domain.Entities.ProductEntities;
using Domain.Entities.ClinicEntities;
using Domain.Entities.DoctorEntites;
using Domain.Entities.AppointmentEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Persistence.Data.Configurations;

namespace Persistence.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
            // options w ctor fady 3shan da ely hay3ml instance mn el Db Package
            // Beyakhod el instance w el object w el initalize 3amtn ely 3amlo fel app settings w el service container
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //    base.OnModelCreating(modelBuilder); base.OnModelCreating =>> lw ba3ml run mn DbContext msh lazm a3mlha 3shan el DbContext ma3ndhash asln fluent api a3mlo run 3shan heya ma3ndhash models wla tables enma lw IDentityContext sa3tha bakteb base 3ady 3shan 3ando models w tables 
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreContext).Assembly);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // de bet3ml execute l ay type bey3ml implement l IEntityTypeConfiguration 

            // Explicitly configure Pet entity in case it wasn't captured by ApplyConfigurationsFromAssembly
            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("Pets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PetName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BirthDate).IsRequired();
                entity.Property(e => e.Gender).IsRequired().HasConversion<byte>();
                entity.Property(e => e.PetType).IsRequired().HasConversion<byte>();
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Avatar).HasMaxLength(255);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            });
            
            // Explicitly configure Clinic entity in case it wasn't captured by ApplyConfigurationsFromAssembly
            modelBuilder.Entity<Clinic>(entity =>
            {
                entity.ToTable("Clinics");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.ClinicName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Status).IsRequired().HasConversion<int>().HasDefaultValue(ClinicStatus.Pending);
                entity.Property(c => c.Rating).HasDefaultValue(0);
                entity.Property(c => c.OwnerId).IsRequired();
                
               /* // Configure relationship with Owner
                entity.HasOne(c => c.Owner)
                    .WithMany()
                    .HasForeignKey(c => c.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
               */
                
                // Configure ClinicAddress as owned entity
                entity.OwnsOne(c => c.Address, addressBuilder => 
                {
                    addressBuilder.Property(a => a.Name).HasMaxLength(100).IsRequired(false);
                    addressBuilder.Property(a => a.Street).HasMaxLength(200).IsRequired(false);
                    addressBuilder.Property(a => a.City).HasMaxLength(100).IsRequired(false);
                    addressBuilder.Property(a => a.Country).HasMaxLength(100).IsRequired(false);
                });
            });
            
            // Explicitly configure Appointment entity
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointments");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.UserId).IsRequired().HasMaxLength(450);
                entity.Property(a => a.AppointmentDate).IsRequired();
                entity.Property(a => a.Status).IsRequired().HasConversion<byte>();
                entity.Property(a => a.Notes).HasMaxLength(500);
                entity.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                
                // Relationships
                entity.HasOne(a => a.PetProfile)
                    .WithMany()
                    .HasForeignKey(a => a.PetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Clinic)
                    .WithMany(c => c.Appointments)
                    .HasForeignKey(a => a.ClinicId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Explicitly configure Rating entity
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Ratings");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.RatingValue).IsRequired();
                entity.Property(r => r.Comment).HasMaxLength(500);
                entity.Property(r => r.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(r => r.UserId).IsRequired().HasMaxLength(450);
                entity.Property(r => r.ClinicId).IsRequired();
                
                // Configure relationship with Clinic
                entity.HasOne(r => r.Clinic)
                    .WithMany(c => c.Ratings)
                    .HasForeignKey(r => r.ClinicId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Explicitly configure WishList entity
            modelBuilder.Entity<WishList>(entity =>
            {
                entity.ToTable("WishLists");
                entity.HasKey(w => w.Id);
                entity.Property(w => w.UserId).IsRequired().HasMaxLength(450);
                entity.Property(w => w.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            });
            
            // Explicitly configure WishListItem entity
            modelBuilder.Entity<WishListItem>(entity =>
            {
                entity.ToTable("WishListItems");
                entity.HasKey(wi => wi.Id);
                entity.Property(wi => wi.AddedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                
                // Configure relationships
                entity.HasOne(wi => wi.WishList)
                    .WithMany(w => w.WishListItems)
                    .HasForeignKey(wi => wi.WishListId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(wi => wi.Product)
                    .WithMany()
                    .HasForeignKey(wi => wi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }


        public DbSet<Product?> Products { get; set; }
        public DbSet<ProductBrand?> ProductBrands { get; set; }
        public DbSet<ProductCategory?> ProductCategories { get; set; }
        public DbSet<OrderEntity?> Orders { get; set; }
        public DbSet<OrderItem?> OrderItems { get; set; }
        public DbSet<DeliveryMethod?> DeliveryMethods { get; set; }
        public DbSet<CustomerBasket> CustomerBaskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
    }
}
