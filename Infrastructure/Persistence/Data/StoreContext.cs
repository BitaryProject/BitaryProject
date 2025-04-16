using Domain.Entities.BasketEntities;
using Domain.Entities.HealthcareEntities;
using Domain.Entities.OrderEntities;
using Domain.Entities.ProductEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        }

        // E-commerce Entities
        public DbSet<Product?> Products { get; set; }
        public DbSet<ProductBrand?> ProductBrands { get; set; }
        public DbSet<ProductCategory?> ProductCategories { get; set; }
        public DbSet<OrderEntity?> Orders { get; set; }
        public DbSet<OrderItem?> OrderItems { get; set; }
        public DbSet<DeliveryMethod?> DeliveryMethods { get; set; }
        public DbSet<CustomerBasket> CustomerBaskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        
        // Healthcare Entities
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PetOwner> PetOwners { get; set; }
        public DbSet<PetProfile> PetProfiles { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
    }
}
