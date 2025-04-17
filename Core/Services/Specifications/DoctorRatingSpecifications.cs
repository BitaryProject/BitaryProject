using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;

namespace Core.Services.Specifications
{
    public class DoctorRatingByDoctorIdSpecification1 : BaseSpecification<DoctorRating>
    {
        public DoctorRatingByDoctorIdSpecification1(Guid doctorId)
            : base(r => r.DoctorId == doctorId)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
            ApplyOrderByDescending(r => r.CreatedDate);
        }
    }
    
    public class DoctorRatingByIdSpecification : BaseSpecification<DoctorRating>
    {
        public DoctorRatingByIdSpecification(Guid id)
            : base(r => r.Id == id)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
        }
    }
    
    public class DoctorRatingPaginatedSpecification1 : BaseSpecification<DoctorRating>
    {
        public DoctorRatingPaginatedSpecification1(int pageIndex, int pageSize)
            : base(null)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
            ApplyOrderByDescending(r => r.CreatedDate);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
    
    public class DoctorRatingByDoctorAndOwnerSpecification1 : BaseSpecification<DoctorRating>
    {
        public DoctorRatingByDoctorAndOwnerSpecification1(Guid doctorId, Guid petOwnerId)
            : base(r => r.DoctorId == doctorId && r.PetOwnerId == petOwnerId)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.PetOwner);
        }
    }
    
    public class ClinicRatingByClinicIdSpecification1 : BaseSpecification<ClinicRating>
    {
        public ClinicRatingByClinicIdSpecification1(Guid clinicId)
            : base(r => r.ClinicId == clinicId)
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
            ApplyOrderByDescending(r => r.CreatedDate);
        }
    }
    
    public class ClinicRatingByIdSpecification : BaseSpecification<ClinicRating>
    {
        public ClinicRatingByIdSpecification(Guid id)
            : base(r => r.Id == id)
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
        }
    }
    
    public class ClinicRatingPaginatedSpecification1 : BaseSpecification<ClinicRating>
    {
        public ClinicRatingPaginatedSpecification1(int pageIndex, int pageSize)
            : base(null)
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
            ApplyOrderByDescending(r => r.CreatedDate);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
    
    public class ClinicRatingByClinicAndOwnerSpecification1 : BaseSpecification<ClinicRating>
    {
        public ClinicRatingByClinicAndOwnerSpecification1(Guid clinicId, Guid petOwnerId)
            : base(r => r.ClinicId == clinicId && r.PetOwnerId == petOwnerId)
        {
            AddInclude(r => r.Clinic);
            AddInclude(r => r.PetOwner);
        }
    }
} 
