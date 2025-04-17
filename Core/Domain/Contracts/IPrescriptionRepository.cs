using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription, Guid>
    {
        Task<Prescription> GetPrescriptionByNumberAsync(string prescriptionNumber);
        Task<IEnumerable<Prescription>> GetPrescriptionsByPetIdAsync(Guid petId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync();
        Task<IEnumerable<Prescription>> GetPrescriptionsByStatusAsync(PrescriptionStatus status);
        Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetPagedPrescriptionsAsync(ISpecification<Prescription> specification, int pageIndex, int pageSize);
    }
} 

