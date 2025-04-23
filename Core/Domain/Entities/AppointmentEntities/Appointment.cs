using Domain.Entities.ClinicEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.PetEntities;
using Domain.Entities.DoctorEntites;
using Domain.Entities.MedicalRecordEntites;

namespace Domain.Entities.AppointmentEntities
{
    public class Appointment : BaseEntity<int>
    {
        public Appointment() { }

        public Appointment(string userId, int petId, int clinicId, int doctorId, DateTime appointmentDate, AppointmentStatus status, string notes)
        {
            UserId = userId;
            PetId = petId;
            ClinicId = clinicId;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            Status = status;
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
        }
        public string UserId { get; set; }

        public int PetId { get; set; }
        public Pet PetProfile { get; set; }

        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
    }
}
