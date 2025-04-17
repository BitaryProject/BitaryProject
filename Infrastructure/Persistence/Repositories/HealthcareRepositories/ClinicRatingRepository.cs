using Core.Domain.Entities.HealthcareEntities;
using Domain.Contracts;
using Infrastructure.Persistence.Data;

namespace Persistence.Repositories
{
    public class ClinicRatingRepository : GenericRepository<ClinicRating>, IClinicRatingRepository
    {
        public ClinicRatingRepository(StoreContext context) : base(context)
        {
        }
    }
} 