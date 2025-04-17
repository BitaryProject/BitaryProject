/*
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Presentation.Controllers
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
            _clinicService = clinicService ?? throw new ArgumentNullException(nameof(clinicService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetAllClinics()
        {
            try
            {
                var clinics = await _clinicService.GetAllClinicsAsync();
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all clinics");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> GetPagedClinics(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedClinics = await _clinicService.GetPagedClinicsAsync(pageIndex, pageSize);
                return Ok(pagedClinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged clinics");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> GetClinicById(Guid id)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(id);

                if (clinic == null)
                    return NotFound($"Clinic with ID {id} not found");

                return Ok(clinic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic {ClinicId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClinicDTO>> CreateClinic(ClinicCreateDTO clinicDto)
        {
            try
            {
                var clinic = await _clinicService.CreateClinicAsync(clinicDto);

                return CreatedAtAction(
                    nameof(GetClinicById),
                    new { id = clinic?.Id },
                    clinic);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clinic");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateClinic(Guid id, ClinicUpdateDTO clinicDto)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(id);

                if (clinic == null)
                    return NotFound($"Clinic with ID {id} not found");

                await _clinicService.UpdateClinicAsync(id, clinicDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic {ClinicId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteClinic(Guid id)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(id);

                if (clinic == null)
                    return NotFound($"Clinic with ID {id} not found");

                await _clinicService.DeleteClinicAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting clinic {ClinicId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> SearchClinics(
            [FromQuery] string searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term cannot be empty");

                var results = await _clinicService.SearchClinicsAsync(searchTerm, pageIndex, pageSize);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clinics with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("city/{city}")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinicsByCity(string city)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city))
                    return BadRequest("City name cannot be empty");

                var clinics = await _clinicService.GetClinicsByCityAsync(city);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinics for city {City}", city);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("postal-code/{postalCode}")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinicsByPostalCode(string postalCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(postalCode))
                    return BadRequest("Postal code cannot be empty");

                var clinics = await _clinicService.GetClinicsByPostalCodeAsync(postalCode);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinics for postal code {PostalCode}", postalCode);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinicsByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest("Status cannot be empty");

                var clinics = await _clinicService.GetClinicsByStatusAsync(status);
                return Ok(clinics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinics with status {Status}", status);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}/update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateClinicStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(id);

                if (clinic == null)
                    return NotFound($"Clinic with ID {id} not found");

                await _clinicService.UpdateClinicStatusAsync(id, status);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for clinic {ClinicId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}/doctors")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetClinicDoctors(Guid id)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(id);
                
                if (clinic == null)
                    return NotFound($"Clinic with ID {id} not found");
                
                var doctors = await _clinicService.GetClinicDoctorsAsync(id);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for clinic {ClinicId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("{clinicId}/doctors/{doctorId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddDoctorToClinic(Guid clinicId, Guid doctorId)
        {
            try
            {
                await _clinicService.AddDoctorToClinicAsync(clinicId, doctorId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding doctor {DoctorId} to clinic {ClinicId}", doctorId, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{clinicId}/doctors/{doctorId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveDoctorFromClinic(Guid clinicId, Guid doctorId)
        {
            try
            {
                await _clinicService.RemoveDoctorFromClinicAsync(clinicId, doctorId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing doctor {DoctorId} from clinic {ClinicId}", doctorId, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetNearbyClinics(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 10)
        {
            try
            {
                var clinics = await _clinicService.GetNearbyClinicsAsync(latitude, longitude, radiusKm);
                return Ok(clinics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nearby clinics");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}


*/
