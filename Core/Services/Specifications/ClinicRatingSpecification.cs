using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;

namespace Core.Services.Specifications
{
    public class ClinicRatingSpecification : BaseSpecification<ClinicRating>
    {
        public ClinicRatingSpecification() : base()
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
        }
    }

    public class ClinicRatingByClinicIdSpecification : BaseSpecification<ClinicRating>
    {
        public ClinicRatingByClinicIdSpecification(Guid clinicId) : base(r => r.ClinicId == clinicId)
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
            AddOrderByDescending(r => r.CreatedDate);
        }
    }

    public class ClinicRatingByClinicAndOwnerSpecification : BaseSpecification<ClinicRating>
    {
        public ClinicRatingByClinicAndOwnerSpecification(Guid clinicId, Guid petOwnerId) 
            : base(r => r.ClinicId == clinicId && r.PetOwnerId == petOwnerId)
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
        }
    }

    public class ClinicRatingPaginatedSpecification : BaseSpecification<ClinicRating>
    {
        public ClinicRatingPaginatedSpecification(int pageIndex, int pageSize) 
            : base()
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
            AddOrderByDescending(r => r.CreatedDate);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
} 
