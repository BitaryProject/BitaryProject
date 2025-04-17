using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;

namespace Core.Services.Implementations.Extensions
{
    /// <summary>
    /// Extension methods for mapping MedicalNote entities to DTOs
    /// </summary>
    public static class MedicalNoteExtensions
    {
        /// <summary>
        /// Converts a MedicalNote entity to a MedicalNoteDTO
        /// </summary>
        /// <param name="note">The MedicalNote entity to convert</param>
        /// <returns>A MedicalNoteDTO representing the entity</returns>
        public static MedicalNoteDTO ToMedicalNoteDTO(this MedicalNote note)
        {
            if (note == null)
                return null;

            return new MedicalNoteDTO
            {
                Id = note.Id,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                MedicalRecordId = note.MedicalRecordId,
                DoctorId = note.DoctorId
            };
        }

        /// <summary>
        /// Converts a collection of MedicalNote entities to MedicalNoteDTOs
        /// </summary>
        /// <param name="notes">The collection of MedicalNote entities to convert</param>
        /// <returns>A collection of MedicalNoteDTOs</returns>
        public static IEnumerable<MedicalNoteDTO> ToMedicalNoteDTOs(this IEnumerable<MedicalNote> notes)
        {
            if (notes == null)
                return Enumerable.Empty<MedicalNoteDTO>();

            return notes.Select(note => note.ToMedicalNoteDTO());
        }

        /// <summary>
        /// Create a summary of a medical note (for quick display)
        /// </summary>
        public static string GetSummary(this MedicalNote note, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(note.Content)) return string.Empty;
            
            if (note.Content.Length <= maxLength)
                return note.Content;
                
            return note.Content.Substring(0, maxLength) + "...";
        }
    }
} 