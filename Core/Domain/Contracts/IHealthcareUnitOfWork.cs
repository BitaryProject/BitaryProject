using System;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IHealthcareUnitOfWork : IDisposable
    {
        IPrescriptionRepository PrescriptionRepository { get; }
        IPetProfileRepository PetProfileRepository { get; }
        IDoctorRepository DoctorRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        IMedicalNoteRepository MedicalNoteRepository { get; }
        IMedicationRepository MedicationRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IClinicRepository ClinicRepository { get; }
        IPetOwnerRepository PetOwnerRepository { get; }
        IDoctorRatingRepository DoctorRatingRepository { get; }
        IClinicRatingRepository ClinicRatingRepository { get; }
        
        // Aliases for backward compatibility
        IPetProfileRepository PetRepository { get => PetProfileRepository; }
        
        Task<int> SaveChangesAsync();
    }
} 