using Domain.Contracts;
using Domain.Entities.MedicalRecordEntites;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class MedicalRecordSpecification : Specifications<MedicalRecord>
    {
        public MedicalRecordSpecification(int id)
            : base(r => r.Id == id)
        {
            AddInclude(r => r.Pet);
            AddInclude(r => r.Doctor);
            AddInclude(r => r.Appointment);
        }

        public MedicalRecordSpecification(int petId, bool includeRelationships = false)
            : base(r => r.PetId == petId)
        {
            if (includeRelationships)
            {
                AddInclude(r => r.Pet);
                AddInclude(r => r.Doctor);
                AddInclude(r => r.Appointment);
            }
            setOrderByDescending(r => r.RecordDate);
        }

        public static MedicalRecordSpecification GetByDoctorId(int doctorId)
        {
            var spec = new MedicalRecordSpecification(r => r.DoctorId == doctorId);
            spec.AddInclude(r => r.Pet);
            spec.setOrderByDescending(r => r.RecordDate);
            return spec;
        }

        public static MedicalRecordSpecification GetByAppointmentId(int appointmentId)
        {
            var spec = new MedicalRecordSpecification(r => r.AppointmentId == appointmentId);
            spec.AddInclude(r => r.Pet);
            spec.AddInclude(r => r.Doctor);
            return spec;
        }

        // Constructor with expression
        private MedicalRecordSpecification(Expression<Func<MedicalRecord, bool>> criteria) 
            : base(criteria)
        {
        }
    }
}
