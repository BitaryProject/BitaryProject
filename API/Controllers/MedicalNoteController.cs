using Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor,Admin")]
    public class MedicalNoteController : ControllerBase
    {
        private readonly IMedicalNoteService _medicalNoteService;
        private readonly ILogger<MedicalNoteController> _logger;

        public MedicalNoteController(
            IMedicalNoteService medicalNoteService,
            ILogger<MedicalNoteController> logger)
        {
            _medicalNoteService = medicalNoteService ?? throw new ArgumentNullException(nameof(medicalNoteService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Adds a note to a medical record
        /// </summary>
        /// <param name="noteDto">The note data</param>
        /// <returns>Status indicating success or failure</returns>
        [HttpPost("addnote")]
        public async Task<IActionResult> AddNoteToMedicalRecord([FromBody] MedicalNotesDTO noteDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for adding note to medical record");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _medicalNoteService.AddNoteToMedicalRecordAsync(noteDto);
                if (result)
                {
                    _logger.LogInformation("Note added successfully to medical record {RecordId}", noteDto.MedicalRecordId);
                    return Ok(new { Message = "Note added successfully" });
                }
                else
                {
                    _logger.LogWarning("Failed to add note to medical record {RecordId}", noteDto.MedicalRecordId);
                    return BadRequest("Failed to add note to medical record");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding note to medical record {RecordId}", noteDto.MedicalRecordId);
                return StatusCode(500, "An error occurred while adding note to medical record");
            }
        }

        /// <summary>
        /// Gets all notes for a medical record
        /// </summary>
        /// <param name="medicalRecordId">The ID of the medical record</param>
        /// <returns>List of notes</returns>
        [HttpGet("record/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<MedicalNoteDTO>>> GetNotesByMedicalRecordId(Guid medicalRecordId)
        {
            try
            {
                var notes = await _medicalNoteService.GetNotesByMedicalRecordIdAsync(medicalRecordId);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notes for medical record {RecordId}", medicalRecordId);
                return StatusCode(500, "An error occurred while retrieving notes");
            }
        }

        /// <summary>
        /// Gets all notes created by a doctor
        /// </summary>
        /// <param name="doctorId">The ID of the doctor</param>
        /// <returns>List of notes</returns>
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<MedicalNoteDTO>>> GetNotesByDoctorId(Guid doctorId)
        {
            try
            {
                var notes = await _medicalNoteService.GetNotesByDoctorIdAsync(doctorId);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notes for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving notes");
            }
        }

        /// <summary>
        /// Gets a specific note by ID
        /// </summary>
        /// <param name="id">The ID of the note</param>
        /// <returns>The note</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalNoteDTO>> GetNoteById(Guid id)
        {
            try
            {
                var note = await _medicalNoteService.GetNoteByIdAsync(id);
                if (note == null)
                {
                    return NotFound($"Note with ID {id} not found");
                }
                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving note {NoteId}", id);
                return StatusCode(500, "An error occurred while retrieving the note");
            }
        }
    }
} 