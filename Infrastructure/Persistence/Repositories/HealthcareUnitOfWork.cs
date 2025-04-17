using Core.Domain.Contracts;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories.HealthcareRepositories;

namespace Infrastructure.Persistence.Repositories
{
    public class HealthcareUnitOfWork : IHealthcareUnitOfWork
    {
        private readonly StoreContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories;
        private bool _disposed;

        public HealthcareUnitOfWork(StoreContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<string, object>();
            _disposed = false;
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
            
        public IMedicalNoteRepository MedicalNoteRepository => 
            GetOrCreateRepository<IMedicalNoteRepository, MedicalNoteRepository>();
            
        public IPrescriptionRepository PrescriptionRepository => 
            GetOrCreateRepository<IPrescriptionRepository, PrescriptionRepository>();
            
        public IMedicationRepository MedicationRepository => 
            GetOrCreateRepository<IMedicationRepository, MedicationRepository>();

        public IDoctorRatingRepository DoctorRatingRepository => 
            GetOrCreateRepository<IDoctorRatingRepository, DoctorRatingRepository>();

        public IClinicRatingRepository ClinicRatingRepository => 
            GetOrCreateRepository<IClinicRatingRepository, ClinicRatingRepository>();

        // Helper method to create repository instances
        private TInterface GetOrCreateRepository<TInterface, TImplementation>()
            where TImplementation : class, TInterface
        {
            return (TInterface)_repositories.GetOrAdd(
                typeof(TInterface).Name, 
                _ => Activator.CreateInstance(typeof(TImplementation), _context)!);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context?.Dispose();
                _disposed = true;
            }
        }
    }
} 