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
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(
            IDoctorService doctorService,
            ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetById(Guid id)
        {
            try
            {
                var doctor = await _doctorService.GetByIdAsync(id);
                return Ok(doctor);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync(pageIndex, pageSize);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all doctors");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetByName(
            string name,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsByNameAsync(name, pageIndex, pageSize);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors with name {Name}", name);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("specialty/{specialty}")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetBySpecialty(
            string specialty,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsBySpecialtyAsync(specialty, pageIndex, pageSize);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors with specialty {Specialty}", specialty);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetByClinic(
            Guid clinicId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsByClinicAsync(clinicId, pageIndex, pageSize);
                return Ok(doctors);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<DoctorDTO>> Create(DoctorCreateUpdateDTO createDto)
        {
            try
            {
                var doctor = await _doctorService.CreateDoctorAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDTO>> Update(Guid id, DoctorCreateUpdateDTO updateDto)
        {
            try
            {
                var doctor = await _doctorService.UpdateDoctorAsync(id, updateDto);
                return Ok(doctor);
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
                _logger.LogError(ex, "Error updating doctor {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 