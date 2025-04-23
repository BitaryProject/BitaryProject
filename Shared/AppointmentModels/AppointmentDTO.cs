using Domain.Entities.AppointmentEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.AppointmentModels
{
    public record AppointmentDTO
    {
        public int Id { get; init; }
        
        public string UserId { get; init; }
        
        [Required(ErrorMessage = "Pet ID is required")]
        public int PetId { get; init; }
        public string PetName { get; init; }
        
        [Required(ErrorMessage = "Clinic ID is required")]
        public int ClinicId { get; init; }
        public string ClinicName { get; init; }
        
        [Required(ErrorMessage = "Doctor ID is required")]
        public int DoctorId { get; init; }
        public string DoctorName { get; init; }
        
        [Required(ErrorMessage = "Appointment date is required")]
        public DateTime AppointmentDate { get; init; }
        
        public AppointmentStatus Status { get; init; }
        public string Notes { get; init; }
        public DateTime CreatedAt { get; init; }
        
        // Method to create a simplified DTO for status updates
        public AppointmentDTO CreateStatusUpdateDTO()
        {
            return new AppointmentDTO
            {
                Id = this.Id,
                Status = this.Status,
                Notes = this.Notes
            };
        }
    }
}
