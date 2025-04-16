using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPrescriptionRepository : IRepositoryBase<Prescription, Guid>
    {
        Task<IEnumerable<Prescription>> GetPrescriptionsByPetProfileIdAsync(Guid petProfileId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Prescription>> GetActivePrescriptionsForPetAsync(Guid petProfileId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId);
        Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetPagedPrescriptionsAsync(
            ISpecification<Prescription> specification, int pageIndex, int pageSize);
    }
} 