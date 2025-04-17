using System;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    /// <summary>
    /// Represents a medical note that can be attached to a medical record
    /// </summary>
    public class MedicalNote : BaseEntity<Guid>
    {
        /// <summary>
        /// The content of the medical note
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// When the note was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Foreign key to the medical record this note belongs to
        /// </summary>
        public Guid MedicalRecordId { get; set; }
        
        /// <summary>
        /// Foreign key to the doctor who created this note
        /// </summary>
        public Guid DoctorId { get; set; }
        
        /// <summary>
        /// Navigation property to the medical record
        /// </summary>
        public MedicalRecord MedicalRecord { get; set; }
        
        /// <summary>
        /// Navigation property to the doctor
        /// </summary>
        public Doctor Doctor { get; set; }
    }
} 
