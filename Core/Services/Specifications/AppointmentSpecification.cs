using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class AppointmentSpecification : BaseSpecification<Appointment>
    {
        public AppointmentSpecification(Guid id) 
            : base(a => a.Id == id)
        {
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(Guid doctorId, int pageIndex, int pageSize)
            : base(a => a.DoctorId == doctorId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(Guid petId, bool forPet, int pageIndex, int pageSize)
            : base(a => a.PetProfileId == petId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(string status, int pageIndex, int pageSize)
            : base(a => a.Status.ToString() == status)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
            : base(a => a.AppointmentDateTime >= startDate && a.AppointmentDateTime <= endDate)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(Guid clinicId, string byClinicParam, int pageIndex, int pageSize)
            : base(a => a.ClinicId == clinicId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(a => a.AppointmentDateTime);
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification(Expression<Func<Appointment, bool>> criteria)
            : base(criteria)
        {
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }

        public AppointmentSpecification()
            : base(null)
        {
            AddOrderBy(a => a.AppointmentDateTime);
            AddInclude(a => a.Doctor);
            AddInclude(a => a.PetProfile);
            AddInclude("PetProfile.Owner");
            AddInclude(a => a.Clinic);
        }
    }
}
