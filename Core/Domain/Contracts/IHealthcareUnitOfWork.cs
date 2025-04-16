using System;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IHealthcareUnitOfWork : IDisposable
    {
        IPrescriptionRepository PrescriptionRepository { get; }
        IPetProfileRepository PetProfileRepository { get; }
        IDoctorRepository DoctorRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        IMedicationRepository MedicationRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IClinicRepository ClinicRepository { get; }
        IPetOwnerRepository PetOwnerRepository { get; }
        
        // Aliases for backward compatibility
        IPetProfileRepository PetRepository { get => PetProfileRepository; }
        
        Task<int> SaveChangesAsync();
    }
} 