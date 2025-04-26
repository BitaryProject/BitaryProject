using Domain.Entities.AppointmentEntities;
using Domain.Entities.DoctorEntites;
using Domain.Entities.SecurityEntities;
using System.Collections.Generic;

namespace Domain.Entities.ClinicEntities
{
    public enum ClinicStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class Clinic : BaseEntity<int>
    {
        public Clinic()
        {
            Doctors = new List<Doctor>();
            Appointments = new List<Appointment>();
            Ratings = new List<Rating>();
            Status = ClinicStatus.Pending;
        }

        public string ClinicName { get; set; }
        public ClinicAddress? Address { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public ClinicStatus Status { get; set; }
        public string OwnerId { get; set; }
      //  public User Owner { get; set; }

        public ICollection<Doctor> Doctors { get; set; } // Must exist
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Rating> Ratings { get; set; }
    }
}