//using Core.Domain.Entities.AppointmentEntities;
//using Core.Domain.Entities.ClinicEntities;
//using Core.Domain.Entities.MedicalRecordEntites;
//using Core.Domain.Entities.SecurityEntities;
//using System.Collections.Generic;

//namespace Core.Domain.Entities.DoctorEntites
//{
//    public class Doctor : BaseEntity<int>
//    {
//        public Doctor()
//        {
//            MedicalRecords = new List<MedicalRecord>();
//            Schedules = new List<DoctorSchedule>();
//        }

//        public string Name { get; set; }
//        public string Specialty { get; set; }
//        public string Email { get; set; }
//        public string Phone { get; set; }
//        public DocGender Gender { get; set; }

//        // Relationships
//        public int ClinicId { get; set; }
//        public Clinic Clinic { get; set; }
//        public string? UserId { get; set; }
//        public User? User { get; set; }

//        public ICollection<MedicalRecord> MedicalRecords { get; set; }
//        public ICollection<DoctorSchedule> Schedules { get; set; }
//        public ICollection<Appointment> Appointments { get; set; }
//    }

//    public enum DocGender : byte
//    {
//        male = 1,
//        female = 2
//    }
//}

