using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.MedicalRecordModels
{
    public record MedicalRecordDTO
    {
        public int Id { get; init; }
        
        [Required(ErrorMessage = "Diagnosis is required")]
        public string Diagnosis { get; init; }
        
        public string Treatment { get; init; }
        
        [Required(ErrorMessage = "Record date is required")]
        public DateTime RecordDate { get; init; }
        
        public string Notes { get; init; }
        
        [Required(ErrorMessage = "Pet ID is required")]
        public int PetId { get; init; }
        public string PetName { get; init; }
        
        [Required(ErrorMessage = "Doctor ID is required")]
        public int DoctorId { get; init; }
        public string DoctorName { get; init; }
        
        [Required(ErrorMessage = "Appointment ID is required")]
        public int AppointmentId { get; init; }
    }
}
