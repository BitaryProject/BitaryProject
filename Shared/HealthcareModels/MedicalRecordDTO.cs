using System;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for medical record
    /// </summary>
    public class MedicalRecordDTO
    {
        public Guid Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        
        // Related entity information
        public string DoctorName { get; set; }
        public Guid DoctorId { get; set; }
        public string PetName { get; set; }
        public Guid PetId { get; set; }
        public string PetOwnerName { get; set; }
    }
    
    /// <summary>
    /// DTO for creating a medical record
    /// </summary>
    public class MedicalRecordCreateDTO
    {
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PetId { get; set; }
    }
    
    /// <summary>
    /// DTO for updating a medical record
    /// </summary>
    public class MedicalRecordUpdateDTO
    {
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
    }
    
    /// <summary>
    /// DTO for time slots
    /// </summary>
    public class TimeSlotDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
} 