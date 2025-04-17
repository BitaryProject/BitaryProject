using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// DTO for retrieving medical notes
    /// </summary>
    public class MedicalNoteDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a new medical note
    /// </summary>
    public class MedicalNoteCreateDTO
    {
        [Required(ErrorMessage = "Note content is required")]
        [StringLength(2000, ErrorMessage = "Note content cannot exceed 2000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Medical record ID is required")]
        public Guid MedicalRecordId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing medical note
    /// </summary>
    public class MedicalNoteUpdateDTO
    {
        [Required(ErrorMessage = "Note content is required")]
        [StringLength(2000, ErrorMessage = "Note content cannot exceed 2000 characters")]
        public string Content { get; set; } = string.Empty;
    }

    public class MedicalNotesDTO
    {
        [Required]
        public Guid MedicalRecordId { get; set; }

        [Required]
        public string Note { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 