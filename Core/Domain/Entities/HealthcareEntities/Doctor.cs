using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Core.Domain.Entities.SecurityEntities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public class Doctor : BaseEntity<Guid>
    {
        public Doctor()
        {
            Clinics = new List<Clinic>();
            Appointments = new List<Appointment>();
            Ratings = new List<DoctorRating>();
            Prescriptions = new List<Prescription>();
            MedicalRecords = new List<MedicalRecord>();
        }

        public string UserId { get; set; }
        public User User { get; set; }
        
        public string Specialization { get; set; }
        public string Biography { get; set; }
        public decimal ConsultationFee { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsVerified { get; set; }
        public bool IsAvailable { get; set; }
        
        // Properties for easier mapping
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public string Bio { get; set; }
        
        // Foreign keys
        public Guid? ClinicId { get; set; }
        
        // Navigation properties
        public ICollection<Clinic> Clinics { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<DoctorRating> Ratings { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public Clinic Clinic { get; set; }
    }
} 

