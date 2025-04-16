using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Threading.Tasks;
using Domain.Exceptions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(
            IMedicalRecordService medicalRecordService,
            ILogger<MedicalRecordsController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecordDTO>> GetById(Guid id)
        {
            try
            {
                var medicalRecord = await _medicalRecordService.GetByIdAsync(id);
                return Ok(medicalRecord);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical record {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetByPet(
            Guid petId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByPetAsync(petId, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical records for pet {PetId}", petId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetByDoctor(
            Guid doctorId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDoctorAsync(doctorId, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical records for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDateRangeAsync(startDate, endDate, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical records between {StartDate} and {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("diagnosis")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetByDiagnosis(
            [FromQuery] string diagnosis, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDiagnosisAsync(diagnosis, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical records with diagnosis {Diagnosis}", diagnosis);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MedicalRecordDTO>> Create(MedicalRecordCreateDTO createDto)
        {
            try
            {
                var medicalRecord = await _medicalRecordService.CreateMedicalRecordAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = medicalRecord.Id }, medicalRecord);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical record");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MedicalRecordDTO>> Update(Guid id, MedicalRecordUpdateDTO updateDto)
        {
            try
            {
                var medicalRecord = await _medicalRecordService.UpdateMedicalRecordAsync(id, updateDto);
                return Ok(medicalRecord);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical record {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _medicalRecordService.DeleteMedicalRecordAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 