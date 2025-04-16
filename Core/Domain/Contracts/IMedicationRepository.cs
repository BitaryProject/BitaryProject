using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IMedicationRepository : IRepositoryBase<Medication, Guid>
    {
        Task<Medication> FindByNameAsync(string name);
        Task<IEnumerable<Medication>> GetMedicationsByCategoryAsync(string category);
        Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm);
    }
} 