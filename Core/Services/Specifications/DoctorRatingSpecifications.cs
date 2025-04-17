using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;

namespace Core.Services.Specifications
{
    // Use the original implementations from DoctorRatingSpecification.cs
    // These are just references to maintain backward compatibility
    
    // **************** DOCTOR RATING SPECIFICATIONS ****************
    
    public class DoctorRatingByDoctorIdSpecification1 : DoctorRatingByDoctorIdSpecification
    {
        public DoctorRatingByDoctorIdSpecification1(Guid doctorId) 
            : base(doctorId)
        {
            // Use base implementation
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
    
    public class DoctorRatingPaginatedSpecification1 : DoctorRatingPaginatedSpecification
    {
        public DoctorRatingPaginatedSpecification1(int pageIndex, int pageSize)
            : base(pageIndex, pageSize)
        {
            // Use base implementation
        }
    }
    
    public class DoctorRatingByDoctorAndOwnerSpecification1 : DoctorRatingByDoctorAndOwnerSpecification
    {
        public DoctorRatingByDoctorAndOwnerSpecification1(Guid doctorId, Guid petOwnerId)
            : base(doctorId, petOwnerId)
        {
            // Use base implementation
        }
    }
    
    // **************** CLINIC RATING SPECIFICATIONS ****************
    
    public class ClinicRatingByClinicIdSpecification1 : ClinicRatingByClinicIdSpecification
    {
        public ClinicRatingByClinicIdSpecification1(Guid clinicId)
            : base(clinicId)
        {
            // Use base implementation
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
    
    public class ClinicRatingPaginatedSpecification1 : ClinicRatingPaginatedSpecification
    {
        public ClinicRatingPaginatedSpecification1(int pageIndex, int pageSize)
            : base(pageIndex, pageSize)
        {
            // Use base implementation
        }
    }
    
    public class ClinicRatingByClinicAndOwnerSpecification1 : ClinicRatingByClinicAndOwnerSpecification
    {
        public ClinicRatingByClinicAndOwnerSpecification1(Guid clinicId, Guid petOwnerId)
            : base(clinicId, petOwnerId)
        {
            // Use base implementation
        }
    }
} 
