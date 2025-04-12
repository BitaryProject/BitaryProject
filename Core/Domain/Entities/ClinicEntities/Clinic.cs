/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.AppointmentEntities;
using Domain.Entities.DoctorEntites;
//using Domain.Entities.SecurityEntities;

namespace Domain.Entities.ClinicEntities
{
    public class Clinic : BaseEntity<Guid>
    {
        public Clinic() { }

        public Clinic(string clinicName, ClinicAddress address, decimal examinationFee , double rating)
        {
            Id = Guid.NewGuid();
            ClinicName = clinicName;
            Address = address;
            ExaminationFee = examinationFee;
            Doctors = new List<Doctor>();
            Rating = rating;
            Appointments = new List<Appointment>();
        }
        public string ClinicName { get; set; }

        public ClinicAddress Address { get; set; }

        public decimal ExaminationFee { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public double Rating { get; set; }
     
    }
}
*/