using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IClinicRepository : IGenericRepository<Clinic, Guid>
    {
        Task<IEnumerable<Clinic>> GetClinicsBySearchTermAsync(string searchTerm);
        Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(Specifications<Clinic> specifications, int pageIndex, int pageSize);
    }
} 