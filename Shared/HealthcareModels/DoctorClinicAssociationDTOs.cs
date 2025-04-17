using System;

namespace Shared.HealthcareModels
{
    public class DoctorClinicAssociationDTO
    {
        public int Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class DoctorClinicAssociationCreateDTO
    {
        public Guid DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    public class DoctorClinicAssociationUpdateDTO
    {
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class AvailableTimeSlotDTO
    {
        public int Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; }
    }

    public class SpecialtyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
} 