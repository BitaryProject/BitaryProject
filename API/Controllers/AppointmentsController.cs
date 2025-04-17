using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Exceptions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPetProfileService _petProfileService;
        private readonly IPetOwnerService _petOwnerService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            IPetProfileService petProfileService,
            IPetOwnerService petOwnerService,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _petProfileService = petProfileService;
            _petOwnerService = petOwnerService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetByIdAsync(id);
                
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(petOwner.Id);
                    bool isPetOwnersPet = false;
                    
                    foreach (var pet in petProfiles)
                    {
                        if (pet.Id == appointment.PetProfileId)
                        {
                            isPetOwnersPet = true;
                            break;
                        }
                    }
                    
                    if (!isPetOwnersPet)
                        return Forbid("You do not have permission to view this appointment");
                }
                else if (User.IsInRole("Doctor"))
                {
                    var userId = User.GetUserId();
                    var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                    
                    if (doctor == null)
                        return NotFound("Doctor profile not found");
                    
                    if (appointment.DoctorId != doctor.Id)
                        return Forbid("You do not have permission to view this appointment");
                }
                
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/my-appointments")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetDoctorAppointments()
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctor.Id);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor appointments");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet-owner/my-appointments")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetPetOwnerAppointments()
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                var appointments = await _appointmentService.GetAppointmentsByOwnerIdAsync(petOwner.Id);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet owner appointments");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<PagedResultDTO<AppointmentDTO>>> GetByDoctor(
            Guid doctorId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId, pageIndex, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<PagedResultDTO<AppointmentDTO>>> GetByPet(
            Guid petId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPetAsync(petId, pageIndex, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for pet {PetId}", petId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<PagedResultDTO<AppointmentDTO>>> GetByClinic(
            Guid clinicId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByClinicAsync(clinicId, pageIndex, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<PagedResultDTO<AppointmentDTO>>> GetByStatus(
            string status, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByStatusAsync(status, pageIndex, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments with status {Status}", status);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<PagedResultDTO<AppointmentDTO>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate, pageIndex, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments between {StartDate} and {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment(AppointmentCreateDTO appointmentDto)
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(appointmentDto.PetProfileId);
                if (petProfile == null || petProfile.OwnerId != petOwner.Id)
                    return Forbid("You can only book appointments for your own pets");
                
                var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointment(Guid id, AppointmentUpdateDTO appointmentDto)
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                var existingAppointment = await _appointmentService.GetByIdAsync(id);
                if (existingAppointment == null)
                    return NotFound($"Appointment with ID {id} not found");
                
                var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(petOwner.Id);
                bool isPetOwnersPet = false;
                
                foreach (var pet in petProfiles)
                {
                    if (pet.Id == existingAppointment.PetProfileId)
                    {
                        isPetOwnersPet = true;
                        break;
                    }
                }
                
                if (!isPetOwnersPet)
                    return Forbid("You can only update appointments for your own pets");
                
                if (existingAppointment.Status != "Scheduled" && existingAppointment.Status != "Pending")
                    return BadRequest("Cannot update appointment that has already been confirmed or completed");
                
                var appointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("status/{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointmentStatus(Guid id, [FromQuery] string status)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var existingAppointment = await _appointmentService.GetByIdAsync(id);
                if (existingAppointment == null)
                    return NotFound($"Appointment with ID {id} not found");
                
                if (existingAppointment.DoctorId != doctor.Id)
                    return Forbid("You can only update status for appointments assigned to you");
                
                var appointment = await _appointmentService.UpdateAppointmentStatusAsync(id, status);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for appointment {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("cancel/{id}")]
        public async Task<ActionResult<bool>> CancelAppointment(Guid id, [FromQuery] string reason)
        {
            try
            {
                var appointment = await _appointmentService.GetByIdAsync(id);
                if (appointment == null)
                    return NotFound($"Appointment with ID {id} not found");
                
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(petOwner.Id);
                    bool isPetOwnersPet = false;
                    
                    foreach (var pet in petProfiles)
                    {
                        if (pet.Id == appointment.PetProfileId)
                        {
                            isPetOwnersPet = true;
                            break;
                        }
                    }
                    
                    if (!isPetOwnersPet)
                        return Forbid("You can only cancel appointments for your own pets");
                }
                else if (User.IsInRole("Doctor"))
                {
                    var userId = User.GetUserId();
                    var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                    
                    if (doctor == null)
                        return NotFound("Doctor profile not found");
                    
                    if (appointment.DoctorId != doctor.Id)
                        return Forbid("You can only cancel appointments assigned to you");
                }
                
                var result = await _appointmentService.CancelAppointmentAsync(id, reason);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling appointment {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor-availability")]
        public async Task<ActionResult<bool>> CheckDoctorAvailability(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime appointmentDateTime,
            [FromQuery] int durationMinutes = 30)
        {
            try
            {
                var isAvailable = await _doctorService.IsDoctorAvailableAsync(
                    doctorId,
                    appointmentDateTime,
                    TimeSpan.FromMinutes(durationMinutes));
                
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking doctor availability");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 