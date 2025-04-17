using Core.Domain.Entities.HealthcareEntities;
using Domain.Contracts;
using Infrastructure.Persistence.Data;

namespace Persistence.Repositories
{
    public class DoctorRatingRepository : GenericRepository<DoctorRating>, IDoctorRatingRepository
    {
        public DoctorRatingRepository(StoreContext context) : base(context)
        {
        }
    }
} 