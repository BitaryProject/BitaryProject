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
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            ILogger<PrescriptionsController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDTO>> GetById(Guid id)
        {
            try
            {
                var prescription = await _prescriptionService.GetByIdAsync(id);
                return Ok(prescription);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescription {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("number/{prescriptionNumber}")]
        public async Task<ActionResult<PrescriptionDTO>> GetByPrescriptionNumber(string prescriptionNumber)
        {
            try
            {
                var prescription = await _prescriptionService.GetByPrescriptionNumberAsync(prescriptionNumber);
                return Ok(prescription);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescription with number {PrescriptionNumber}", prescriptionNumber);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<PagedResultDTO<PrescriptionDTO>>> GetByPet(
            Guid petId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByPetAsync(petId, pageIndex, pageSize);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescriptions for pet {PetId}", petId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<PagedResultDTO<PrescriptionDTO>>> GetByDoctor(
            Guid doctorId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByDoctorAsync(doctorId, pageIndex, pageSize);
                return Ok(prescriptions);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescriptions for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<PagedResultDTO<PrescriptionDTO>>> GetByStatus(
            string status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByStatusAsync(status, pageIndex, pageSize);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescriptions with status {Status}", status);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<PagedResultDTO<PrescriptionDTO>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByDateRangeAsync(startDate, endDate, pageIndex, pageSize);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescriptions between {StartDate} and {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("medication/{medicationName}")]
        public async Task<ActionResult<PagedResultDTO<PrescriptionDTO>>> GetByMedication(
            string medicationName, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByMedicationAsync(medicationName, pageIndex, pageSize);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prescriptions for medication {MedicationName}", medicationName);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PrescriptionDTO>> Create(PrescriptionCreateDTO createDto)
        {
            try
            {
                var prescription = await _prescriptionService.CreatePrescriptionAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescription);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PrescriptionDTO>> Update(Guid id, PrescriptionUpdateDTO updateDto)
        {
            try
            {
                var prescription = await _prescriptionService.UpdatePrescriptionAsync(id, updateDto);
                return Ok(prescription);
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
                _logger.LogError(ex, "Error updating prescription {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPatch("{id}/status/{status}")]
        public async Task<ActionResult<PrescriptionDTO>> UpdateStatus(Guid id, string status)
        {
            try
            {
                var prescription = await _prescriptionService.UpdatePrescriptionStatusAsync(id, status);
                return Ok(prescription);
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
                _logger.LogError(ex, "Error updating status for prescription {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _prescriptionService.DeletePrescriptionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prescription {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("check-medication-safety")]
        public async Task<ActionResult<bool>> CheckMedicationSafety(
            [FromQuery] Guid petId,
            [FromQuery] Guid medicationId)
        {
            try
            {
                var isSafe = await _prescriptionService.IsMedicationSafeForPetAsync(petId, medicationId);
                return Ok(isSafe);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking medication safety for pet {PetId} and medication {MedicationId}", petId, medicationId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 