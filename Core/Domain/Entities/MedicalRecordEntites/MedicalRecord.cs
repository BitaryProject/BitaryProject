/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.PetEntities;
using Domain.Entities.DoctorEntites;
using Domain.Entities.AppointmentEntities;
namespace Domain.Entities.MedicalRecordEntites
{
    public class MedicalRecord : BaseEntity<Guid>
    {
        public MedicalRecord() { }

        public MedicalRecord(Guid petId, Guid doctorId, string diagnosis, string treatment, DateTime recordDate, string notes , Guid appointmentId)
        {
            Id = Guid.NewGuid();
            PetId = petId;
            DoctorId = doctorId;
            Diagnosis = diagnosis;
            Treatment = treatment;
            RecordDate = recordDate;
            Notes = notes;
            AppointmentId = appointmentId;
        }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime RecordDate { get; set; }
        public string Notes { get; set; }
        public Guid PetId { get; set; }
        public  Pet PetProfile { get; set; }
        public Guid DoctorId { get; set; }
        public  Doctor Doctor { get; set; }
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}

*/