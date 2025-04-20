using System;
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
    public class MedicalRecord : BaseEntity<int>
    {
        public MedicalRecord() { }

        public MedicalRecord(int petId, int doctorId, string diagnosis, string treatment, DateTime recordDate, string notes, int appointmentId)
        {
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
        public int PetId { get; set; }
        public Pet Pet { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}

