using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.HealthcareModels;

namespace Core.Services.Abstractions
{
    /// <summary>
    /// Interface for service that handles medical notes
    /// </summary>
    public interface IMedicalNoteService
    {
        /// <summary>
        /// Adds a note to a medical record
        /// </summary>
        /// <param name="noteDto">The note data transfer object</param>
        /// <returns>True if the note was added successfully, false otherwise</returns>
        Task<bool> AddNoteToMedicalRecordAsync(MedicalNotesDTO noteDto);
        
        /// <summary>
        /// Gets notes by medical record ID
        /// </summary>
        /// <param name="medicalRecordId">The medical record ID</param>
        /// <returns>A list of medical notes</returns>
        Task<IEnumerable<MedicalNoteDTO>> GetNotesByMedicalRecordIdAsync(Guid medicalRecordId);
        
        /// <summary>
        /// Gets notes by doctor ID
        /// </summary>
        /// <param name="doctorId">The doctor ID</param>
        /// <returns>A list of medical notes</returns>
        Task<IEnumerable<MedicalNoteDTO>> GetNotesByDoctorIdAsync(Guid doctorId);
        
        /// <summary>
        /// Gets a medical note by ID
        /// </summary>
        /// <param name="id">The note ID</param>
        /// <returns>The medical note, or null if not found</returns>
        Task<MedicalNoteDTO> GetNoteByIdAsync(Guid id);
    }
} 