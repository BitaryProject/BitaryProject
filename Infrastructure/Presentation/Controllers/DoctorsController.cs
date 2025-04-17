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
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(
            IDoctorService doctorService,
            ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all doctors");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetPagedDoctors(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedDoctors = await _doctorService.GetPagedDoctorsAsync(pageIndex, pageSize);
                return Ok(pagedDoctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged doctors");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(Guid id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor {DoctorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDTO>> CreateDoctor(DoctorCreateDTO doctorDto)
        {
            try
            {
                var doctor = await _doctorService.CreateDoctorAsync(doctorDto);

                return CreatedAtAction(
                    nameof(GetDoctorById),
                    new { id = doctor.Id },
                    doctor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> UpdateDoctor(Guid id, DoctorUpdateDTO doctorDto)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                // If the user is a doctor, verify they are updating their own profile
                // Implement additional authorization logic here

                await _doctorService.UpdateDoctorAsync(id, doctorDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor {DoctorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDoctor(Guid id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                await _doctorService.DeleteDoctorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor {DoctorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("specialty/{specialty}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctorsBySpecialty(string specialty)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(specialty))
                    return BadRequest("Specialty cannot be empty");

                var doctors = await _doctorService.GetDoctorsBySpecialtyAsync(specialty);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors with specialty {Specialty}", specialty);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctorsByClinicId(Guid clinicId)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsByClinicIdAsync(clinicId);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAvailableDoctors(
            [FromQuery] DateTime date,
            [FromQuery] Guid clinicId )
        {
            try
            {
                var doctors = await _doctorService.GetAvailableDoctorsAsync(date, clinicId);
                return Ok(doctors);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available doctors for date {Date}", date);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateDoctorStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                await _doctorService.UpdateDoctorStatusAsync(id, status);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for doctor {DoctorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}/clinic/{clinicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignDoctorToClinic(Guid id, Guid clinicId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                await _doctorService.AssignDoctorToClinicAsync(id, clinicId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning doctor {DoctorId} to clinic {ClinicId}", id, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> SearchDoctors(
            [FromQuery] string searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term cannot be empty");

                var results = await _doctorService.SearchDoctorsAsync(searchTerm, pageIndex, pageSize);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 

