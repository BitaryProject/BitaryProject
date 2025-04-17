/*
using Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infrastructure.Presentation.Extensions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Exceptions;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorClinicController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;
        private readonly ILogger<DoctorClinicController> _logger;

        public DoctorClinicController(
            IDoctorService doctorService,
            IClinicService clinicService,
            ILogger<DoctorClinicController> logger)
        {
            _doctorService = doctorService;
            _clinicService = clinicService;
            _logger = logger;
        }

        [HttpGet("doctors/{clinicId}")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetDoctorsByClinic(
            Guid clinicId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");
                
                var doctors = await _doctorService.GetDoctorsByClinicAsync(clinicId, pageIndex, pageSize);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctors/by-clinic/{clinicId:int}")]
        public async Task<ActionResult<PagedResultDTO<DoctorDTO>>> GetDoctorsByClinicAsync(
            int clinicId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(new Guid(clinicId.ToString()));
                
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");
                
                var doctors = await _doctorService.GetDoctorsByClinicAsync(clinicId, pageIndex, pageSize);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinics/{doctorId}")]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> GetClinicsByDoctor(
            Guid doctorId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");
                
                var clinics = await _clinicService.GetClinicsByDoctorAsync(doctorId, pageIndex, pageSize);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinics for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinics/by-doctor/{doctorId:int}")]
        public async Task<ActionResult<PagedResultDTO<ClinicDTO>>> GetClinicsByDoctorAsync(
            int doctorId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(new Guid(doctorId.ToString()));
                
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");
                
                var clinics = await _clinicService.GetClinicsByDoctorAsync(new Guid(doctorId.ToString()), pageIndex, pageSize);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinics for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("associate")]
        [Authorize(Roles = "Admin,ClinicAdmin")]
        public async Task<ActionResult> AssociateDoctorWithClinic(DoctorClinicAssociationCreateDTO associationDto)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(associationDto.DoctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {associationDto.DoctorId} not found");
                
                var clinic = await _clinicService.GetClinicByIdAsync(associationDto.ClinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {associationDto.ClinicId} not found");
                
                if (User.IsInRole("ClinicAdmin"))
                {
                    var userId = User.GetUserId();
                    var isAdmin = await _clinicService.IsUserClinicAdminAsync(userId, associationDto.ClinicId);
                    
                    if (!isAdmin)
                        return Forbid("You are not authorized to associate doctors with this clinic");
                }
                
                await _doctorService.AssociateDoctorWithClinicAsync(associationDto.DoctorId, associationDto.ClinicId);
                return Ok();
            }
            catch (DuplicateAssociationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error associating doctor {DoctorId} with clinic {ClinicId}", 
                    associationDto.DoctorId, associationDto.ClinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("dissociate")]
        [Authorize(Roles = "Admin,ClinicAdmin")]
        public async Task<ActionResult> DissociateDoctorFromClinic(DoctorClinicAssociationUpdateDTO associationDto)
        {
            try
            {
                // For this operation, we need both DoctorId and ClinicId, which are not in the UpdateDTO
                // We'll extract from query parameters
                var doctorIdStr = Request.Query["doctorId"].ToString();
                var clinicIdStr = Request.Query["clinicId"].ToString();
                
                if (string.IsNullOrEmpty(doctorIdStr) || string.IsNullOrEmpty(clinicIdStr))
                {
                    return BadRequest("Both doctorId and clinicId must be provided");
                }
                
                if (!Guid.TryParse(doctorIdStr, out var doctorId))
                {
                    return BadRequest("Invalid doctorId format");
                }
                
                if (!Guid.TryParse(clinicIdStr, out var clinicId))
                {
                    return BadRequest("Invalid clinicId format");
                }
                
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");
                
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");
                
                if (User.IsInRole("ClinicAdmin"))
                {
                    var userId = User.GetUserId();
                    var isAdmin = await _clinicService.IsUserClinicAdminAsync(userId, clinicId);
                    
                    if (!isAdmin)
                        return Forbid("You are not authorized to dissociate doctors from this clinic");
                }
                
                await _doctorService.DissociateDoctorFromClinicAsync(doctorId, clinicId);
                return Ok();
            }
            catch (AssociationNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dissociating doctor from clinic");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor-specialties/{clinicId}")]
        public async Task<ActionResult<PagedResultDTO<SpecialtyDTO>>> GetSpecialtiesInClinic(
            Guid clinicId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");
                
                var specialties = await _doctorService.GetSpecialtiesByClinicAsync(clinicId, pageIndex, pageSize);
                return Ok(specialties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving specialties for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("available-time-slots")]
        public async Task<ActionResult<PagedResultDTO<AvailableTimeSlotDTO>>> GetAvailableTimeSlots(
            [FromQuery] Guid doctorId, 
            [FromQuery] Guid clinicId, 
            [FromQuery] DateTime? fromDate,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return NotFound($"Doctor with ID {doctorId} not found");
                
                var clinic = await _clinicService.GetClinicByIdAsync(clinicId);
                if (clinic == null)
                    return NotFound($"Clinic with ID {clinicId} not found");
                
                var isAssociated = await _doctorService.IsDoctorAssociatedWithClinicAsync(doctorId, clinicId);
                if (!isAssociated)
                    return BadRequest("The doctor is not associated with the specified clinic");
                
                var timeSlots = await _doctorService.GetAvailableTimeSlotsAsync(
                    doctorId, 
                    clinicId, 
                    fromDate ?? DateTime.Today, 
                    pageIndex, 
                    pageSize);
                    
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slots for doctor {DoctorId} at clinic {ClinicId}", 
                    doctorId, clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
*/

