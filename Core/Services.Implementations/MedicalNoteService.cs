using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Contracts;
using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Abstractions;
using Core.Services.Implementations.Extensions;
using Microsoft.Extensions.Logging;
using Shared.HealthcareModels;
using Core.Common.Specifications;

namespace Core.Services.Implementations
{
    public class MedicalNoteService : IMedicalNoteService
    {
        private readonly IHealthcareUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalNoteService> _logger;

        public MedicalNoteService(
            IHealthcareUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MedicalNoteService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AddNoteToMedicalRecordAsync(MedicalNotesDTO noteDto)
        {
            if (noteDto == null)
            {
                throw new ArgumentNullException(nameof(noteDto));
            }

            _logger.LogInformation($"Adding note to medical record with ID: {noteDto.MedicalRecordId}");

            var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetByIdAsync(noteDto.MedicalRecordId);

            if (medicalRecord == null)
            {
                _logger.LogWarning($"Medical record with ID {noteDto.MedicalRecordId} not found");
                throw new ApplicationException($"Medical record with ID {noteDto.MedicalRecordId} not found");
            }

            // Create a new MedicalNote entity
            var medicalNote = new MedicalNote
            {
                Content = noteDto.Note,
                CreatedAt = DateTime.UtcNow,
                MedicalRecordId = noteDto.MedicalRecordId,
                DoctorId = medicalRecord.DoctorId // Assuming the doctor who created the record is adding the note
            };

            // Add the note to the database
            await _unitOfWork.MedicalNoteRepository.AddAsync(medicalNote);
            
            // Also append to the Notes field for backward compatibility
            if (string.IsNullOrEmpty(medicalRecord.Notes))
            {
                medicalRecord.Notes = $"[{DateTime.UtcNow}] {noteDto.Note}";
            }
            else
            {
                medicalRecord.Notes += $"\n[{DateTime.UtcNow}] {noteDto.Note}";
            }

            _unitOfWork.MedicalRecordRepository.Update(medicalRecord);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Note added successfully to medical record with ID: {noteDto.MedicalRecordId}");

            return true;
        }

        public async Task<IEnumerable<MedicalNoteDTO>> GetNotesByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            _logger.LogInformation($"Getting notes for medical record with ID: {medicalRecordId}");

            var notes = await _unitOfWork.MedicalNoteRepository.GetNotesByMedicalRecordIdAsync(medicalRecordId);
            return notes.ToMedicalNoteDTOs();
        }

        public async Task<IEnumerable<MedicalNoteDTO>> GetNotesByDoctorIdAsync(Guid doctorId)
        {
            _logger.LogInformation($"Getting notes created by doctor with ID: {doctorId}");

            var notes = await _unitOfWork.MedicalNoteRepository.GetNotesByDoctorIdAsync(doctorId);
            return notes.ToMedicalNoteDTOs();
        }

        public async Task<MedicalNoteDTO> GetNoteByIdAsync(Guid id)
        {
            _logger.LogInformation($"Getting note with ID: {id}");

            var note = await _unitOfWork.MedicalNoteRepository.GetByIdAsync(id);
            if (note == null)
            {
                _logger.LogWarning($"Note with ID {id} not found");
                return null;
            }

            return note.ToMedicalNoteDTO();
        }
    }
} 