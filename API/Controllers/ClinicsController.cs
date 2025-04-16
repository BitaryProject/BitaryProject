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
    public class ClinicsController : ControllerBase
    {
        private readonly IClinicService _clinicService;
        private readonly ILogger<ClinicsController> _logger;

        public ClinicsController(
            IClinicService clinicService,
            ILogger<ClinicsController> logger)
        {
            _clinicService = clinicService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> GetById(Guid id)
        {
            try
            {
                var clinic = await _clinicService.GetByIdAsync(id);
                return Ok(clinic);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinic {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> GetAll(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinics = await _clinicService.GetAllClinicsAsync(pageIndex, pageSize);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all clinics");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> GetByName(
            string name, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinics = await _clinicService.GetClinicsByNameAsync(name, pageIndex, pageSize);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinics with name {Name}", name);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("address/{address}")]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> GetByAddress(
            string address, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinics = await _clinicService.GetClinicsByAddressAsync(address, pageIndex, pageSize);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinics with address {Address}", address);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClinicDTO>> Create(ClinicCreateUpdateDTO createDto)
        {
            try
            {
                var clinic = await _clinicService.CreateClinicAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, clinic);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clinic");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClinicDTO>> Update(Guid id, ClinicCreateUpdateDTO updateDto)
        {
            try
            {
                var clinic = await _clinicService.UpdateClinicAsync(id, updateDto);
                return Ok(clinic);
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
                _logger.LogError(ex, "Error updating clinic {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _clinicService.DeleteClinicAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting clinic {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 