using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;

namespace Core.Services.Specifications
{
    public class DoctorRatingSpecification : BaseSpecification<DoctorRating>
    {
        public DoctorRatingSpecification() : base()
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
        }
    }

    public class DoctorRatingByDoctorIdSpecification : BaseSpecification<DoctorRating>
    {
        public DoctorRatingByDoctorIdSpecification(Guid doctorId) : base(r => r.DoctorId == doctorId)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
            AddOrderByDescending(r => r.CreatedDate);
        }
    }

    public class DoctorRatingByDoctorAndOwnerSpecification : BaseSpecification<DoctorRating>
    {
        public DoctorRatingByDoctorAndOwnerSpecification(Guid doctorId, Guid petOwnerId) 
            : base(r => r.DoctorId == doctorId && r.PetOwnerId == petOwnerId)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
        }
    }

    public class DoctorRatingPaginatedSpecification : BaseSpecification<DoctorRating>
    {
        public DoctorRatingPaginatedSpecification(int pageIndex, int pageSize) 
            : base()
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
            AddOrderByDescending(r => r.CreatedDate);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
} 
