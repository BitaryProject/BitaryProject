using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts;

namespace Core.Domain.Contracts
{
    public interface IClinicRatingRepository : IGenericRepository<ClinicRating, Guid>
    {
    }
} 

