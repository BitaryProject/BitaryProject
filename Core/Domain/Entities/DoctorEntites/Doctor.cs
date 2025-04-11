using Domain.Entities.ClinicEntities;
using Domain.Entities.MedicalRecordEntites;
using Domain.Entities.SecurityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DoctorEntites
{

    public enum DocGender : byte
    {
        male = 1,
        female = 2,
        m = 1,
        f = 2
    }
    public class Doctor : BaseEntity<Guid>
    {
        public Doctor() { }

        public Doctor(string name, string specialty, string email, string phone, DocGender gender, Guid clinicId )
        {
            Id = Guid.NewGuid();
            Name = name;
            Specialty = specialty;
            Email = email;
            Phone = phone;
            Gender = gender;
            ClinicId = clinicId;
            MedicalRecords = new List<MedicalRecord>();
            Schedules = new List<DoctorSchedule>();
        }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DocGender Gender { get; set; }
        public Guid ClinicId { get; set; }
        public  Clinic Clinic { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();


    }

}
