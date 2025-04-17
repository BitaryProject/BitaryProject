using Core.Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IMedicationRepository : IGenericRepository<Medication, Guid>
    {
        Task<IEnumerable<Medication>> GetMedicationsBySearchTermAsync(string searchTerm);
        Task<IEnumerable<Medication>> GetMedicationsByCategoryAsync(string category);
        Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm);
        Task<Medication> FindByNameAsync(string name);
    }
} 

