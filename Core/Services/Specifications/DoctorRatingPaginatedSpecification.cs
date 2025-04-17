using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;

namespace Core.Services.Specifications
{
    // Comment out or rename these duplicate classes
    // public class DoctorRatingPaginatedSpecification : BaseSpecification<DoctorRating>
    // {
    //     public DoctorRatingPaginatedSpecification(int skip, int take)
    //     {
    //         ApplyPaging(skip, take);
    //         AddInclude(r => r.Doctor);
    //         AddInclude(r => r.PetOwner);
    //         ApplyOrderByDescending(r => r.CreatedAt);
    //     }
    // }

    // public class DoctorRatingByDoctorIdSpecification : BaseSpecification<DoctorRating>
    // {
    //     public DoctorRatingByDoctorIdSpecification(Guid doctorId)
    //     {
    //         AddCriteria(r => r.DoctorId == doctorId);
    //         AddInclude(r => r.PetOwner);
    //         ApplyOrderByDescending(r => r.CreatedAt);
    //     }
    // }

    // public class DoctorRatingByDoctorAndOwnerSpecification : BaseSpecification<DoctorRating>
    // {
    //     public DoctorRatingByDoctorAndOwnerSpecification(Guid doctorId, Guid petOwnerId)
    //     {
    //         AddCriteria(r => r.DoctorId == doctorId && r.PetOwnerId == petOwnerId);
    //     }
    // }
} 
