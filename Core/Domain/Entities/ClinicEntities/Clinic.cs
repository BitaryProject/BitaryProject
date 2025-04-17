//using Core.Domain.Entities.AppointmentEntities;
//using Core.Domain.Entities.DoctorEntites;
//using System.Collections.Generic;

//namespace Core.Domain.Entities.ClinicEntities
//{
//    public class Clinic : BaseEntity<int>
//    {
//        public Clinic()
//        {
//            Doctors = new List<Doctor>();
//            Appointments = new List<Appointment>();
//        }

//        public string ClinicName { get; set; }
//        public ClinicAddress Address { get; set; }
//        public decimal ExaminationFee { get; set; }
//        public double Rating { get; set; }

//        public ICollection<Doctor> Doctors { get; set; } // Must exist
//        public ICollection<Appointment> Appointments { get; set; }
//    }

 
//}

