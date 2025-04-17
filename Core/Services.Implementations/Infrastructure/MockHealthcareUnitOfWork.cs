using System;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Services.Implementations.Infrastructure.Repositories;

namespace Core.Services.Implementations.Infrastructure
{
    /// <summary>
    /// Mock implementation of IHealthcareUnitOfWork for testing without database dependencies
    /// </summary>
    public class MockHealthcareUnitOfWork : IHealthcareUnitOfWork
    {
        private readonly Lazy<IMedicalNoteRepository> _medicalNoteRepository;
        private readonly Lazy<IMedicalRecordRepository> _medicalRecordRepository;
        private readonly Lazy<IDoctorRepository> _doctorRepository;
        private readonly Lazy<IClinicRepository> _clinicRepository;
        private readonly Lazy<IPetProfileRepository> _petProfileRepository;
        private readonly Lazy<IPetOwnerRepository> _petOwnerRepository;
        private readonly Lazy<IPrescriptionRepository> _prescriptionRepository;
        private readonly Lazy<IMedicationRepository> _medicationRepository;
        private readonly Lazy<IAppointmentRepository> _appointmentRepository;
        private readonly Lazy<IDoctorRatingRepository> _doctorRatingRepository;
        private readonly Lazy<IClinicRatingRepository> _clinicRatingRepository;

        public MockHealthcareUnitOfWork()
        {
            _medicalNoteRepository = new Lazy<IMedicalNoteRepository>(() => new MockMedicalNoteRepository());
            _medicalRecordRepository = new Lazy<IMedicalRecordRepository>(() => new MockMedicalRecordRepository());
            _doctorRepository = new Lazy<IDoctorRepository>(() => new MockRepository<IDoctorRepository>());
            _clinicRepository = new Lazy<IClinicRepository>(() => new MockRepository<IClinicRepository>());
            _petProfileRepository = new Lazy<IPetProfileRepository>(() => new MockRepository<IPetProfileRepository>());
            _petOwnerRepository = new Lazy<IPetOwnerRepository>(() => new MockRepository<IPetOwnerRepository>());
            _prescriptionRepository = new Lazy<IPrescriptionRepository>(() => new MockRepository<IPrescriptionRepository>());
            _medicationRepository = new Lazy<IMedicationRepository>(() => new MockRepository<IMedicationRepository>());
            _appointmentRepository = new Lazy<IAppointmentRepository>(() => new MockRepository<IAppointmentRepository>());
            _doctorRatingRepository = new Lazy<IDoctorRatingRepository>(() => new MockRepository<IDoctorRatingRepository>());
            _clinicRatingRepository = new Lazy<IClinicRatingRepository>(() => new MockRepository<IClinicRatingRepository>());
        }

        public IMedicalNoteRepository MedicalNoteRepository => _medicalNoteRepository.Value;
        public IMedicalRecordRepository MedicalRecordRepository => _medicalRecordRepository.Value;
        public IDoctorRepository DoctorRepository => _doctorRepository.Value;
        public IClinicRepository ClinicRepository => _clinicRepository.Value;
        public IPetProfileRepository PetProfileRepository => _petProfileRepository.Value;
        public IPetOwnerRepository PetOwnerRepository => _petOwnerRepository.Value;
        public IPrescriptionRepository PrescriptionRepository => _prescriptionRepository.Value;
        public IMedicationRepository MedicationRepository => _medicationRepository.Value;
        public IAppointmentRepository AppointmentRepository => _appointmentRepository.Value;
        public IDoctorRatingRepository DoctorRatingRepository => _doctorRatingRepository.Value;
        public IClinicRatingRepository ClinicRatingRepository => _clinicRatingRepository.Value;
        
        public IPetProfileRepository PetRepository => PetProfileRepository;

        public async Task<int> SaveChangesAsync()
        {
            // Just return a success value for mock implementation
            return await Task.FromResult(1);
        }

        public void Dispose()
        {
            // No resources to dispose in the mock implementation
            GC.SuppressFinalize(this);
        }
    }
} 