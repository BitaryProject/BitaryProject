using Domain.Contracts;
using Domain.Entities;
using Persistence.Data;
using Persistence.Repositories.HealthcareRepositories;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class HealthcareUnitOfWork : IHealthcareUnitOfWork
    {
        private readonly StoreContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public HealthcareUnitOfWork(StoreContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<string, object>();
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        // Property implementations for repositories
        public IClinicRepository ClinicRepository => 
            GetOrCreateRepository<IClinicRepository, ClinicRepository>();
            
        public IDoctorRepository DoctorRepository => 
            GetOrCreateRepository<IDoctorRepository, DoctorRepository>();
            
        public IPetOwnerRepository PetOwnerRepository => 
            GetOrCreateRepository<IPetOwnerRepository, PetOwnerRepository>();
            
        public IPetProfileRepository PetProfileRepository => 
            GetOrCreateRepository<IPetProfileRepository, PetProfileRepository>();
            
        public IAppointmentRepository AppointmentRepository => 
            GetOrCreateRepository<IAppointmentRepository, AppointmentRepository>();
            
        public IMedicalRecordRepository MedicalRecordRepository => 
            GetOrCreateRepository<IMedicalRecordRepository, MedicalRecordRepository>();
            
        public IPrescriptionRepository PrescriptionRepository => 
            GetOrCreateRepository<IPrescriptionRepository, PrescriptionRepository>();

        // Helper method to create repository instances
        private TInterface GetOrCreateRepository<TInterface, TImplementation>()
            where TImplementation : class, TInterface
        {
            return (TInterface)_repositories.GetOrAdd(
                typeof(TInterface).Name, 
                _ => Activator.CreateInstance(typeof(TImplementation), _context)!);
        }
    }
} 