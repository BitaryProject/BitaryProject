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
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService _medicationService;
        private readonly ILogger<MedicationsController> _logger;

        public MedicationsController(
            IMedicationService medicationService,
            ILogger<MedicationsController> logger)
        {
            _medicationService = medicationService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicationDTO>> GetById(Guid id)
        {
            try
            {
                var medication = await _medicationService.GetByIdAsync(id);
                return Ok(medication);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medication {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PagedResultDTO<MedicationDTO>>> GetByName(
            string name, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var medications = await _medicationService.GetMedicationsByNameAsync(name, pageIndex, pageSize);
                return Ok(medications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medications with name {Name}", name);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("dosageform/{dosageForm}")]
        public async Task<ActionResult<PagedResultDTO<MedicationDTO>>> GetByDosageForm(
            string dosageForm, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var medications = await _medicationService.GetMedicationsByDosageFormAsync(dosageForm, pageIndex, pageSize);
                return Ok(medications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medications with dosage form {DosageForm}", dosageForm);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("manufacturer/{manufacturer}")]
        public async Task<ActionResult<PagedResultDTO<MedicationDTO>>> GetByManufacturer(
            string manufacturer, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var medications = await _medicationService.GetMedicationsByManufacturerAsync(manufacturer, pageIndex, pageSize);
                return Ok(medications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medications from manufacturer {Manufacturer}", manufacturer);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pricerange")]
        public async Task<ActionResult<PagedResultDTO<MedicationDTO>>> GetByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var medications = await _medicationService.GetMedicationsByPriceRangeAsync(minPrice, maxPrice, pageIndex, pageSize);
                return Ok(medications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medications with price between {MinPrice} and {MaxPrice}", minPrice, maxPrice);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MedicationDTO>> Create(MedicationCreateDTO createDto)
        {
            try
            {
                var medication = await _medicationService.CreateMedicationAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medication);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medication");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MedicationDTO>> Update(Guid id, MedicationUpdateDTO updateDto)
        {
            try
            {
                var medication = await _medicationService.UpdateMedicationAsync(id, updateDto);
                return Ok(medication);
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
                _logger.LogError(ex, "Error updating medication {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _medicationService.DeleteMedicationAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medication {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 