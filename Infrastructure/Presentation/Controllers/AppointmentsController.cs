using Infrastructure.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services.Abstractions;
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
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
            _petProfileService = petProfileService ?? throw new ArgumentNullException(nameof(petProfileService));
            _petOwnerService = petOwnerService ?? throw new ArgumentNullException(nameof(petOwnerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAllAppointments()
        {
            try
            {
                var doctorAppointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(Guid.Empty);
                return Ok(doctorAppointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all appointments");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HealthcarePagedResultDTO<AppointmentDTO>>> GetPagedAppointments(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctorId = Guid.Empty;
                var pagedAppointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId, pageIndex, pageSize);
                return Ok(pagedAppointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged appointments");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return Ok(appointment);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Policy = "IsAuthenticated")]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment([FromBody] AppointmentCreateDTO appointmentDto)
        {
            try
            {
                // Verify pet owner access if request comes from a pet owner
                if (User.IsInRole("PetOwner"))
                {
                    var petOwnerId = User.GetUserId();
                    if (!await _appointmentService.IsPetOwnedByUserAsync(appointmentDto.PetProfileId, petOwnerId))
                    {
                        return Forbid("You can only schedule appointments for your own pets.");
                    }
                }
                
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
                return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsAuthenticated")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointment(Guid id, [FromBody] AppointmentUpdateDTO appointmentDto)
        {
            try
            {
                // Verify permission to update this appointment
                if (User.IsInRole("PetOwner"))
                {
                    var petOwnerId = User.GetUserId();
                    var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                    if (!await _appointmentService.IsPetOwnedByUserAsync(appointment.PetProfileId, petOwnerId))
                    {
                        return Forbid("You can only update appointments for your own pets.");
                    }
                }
                
                var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
                return Ok(updatedAppointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult> DeleteAppointment(Guid id)
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
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByPetId(Guid petId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPetIdAsync(petId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for pet {PetId}", petId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByDoctorId(Guid doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByClinicId(Guid clinicId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByClinicIdAsync(clinicId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for clinic {ClinicId}", clinicId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<HealthcarePagedResultDTO<AppointmentDTO>>> GetAppointmentsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments between {StartDate} and {EndDate}", 
                    startDate.ToShortDateString(), endDate.ToShortDateString());
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<HealthcarePagedResultDTO<AppointmentDTO>>> GetAppointmentsByStatus(
            string status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByStatusAsync(status, pageNumber, pageSize);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments with status {Status}", status);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Policy = "IsAuthenticated")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointmentStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var updatedAppointment = await _appointmentService.UpdateAppointmentStatusAsync(id, status);
                return Ok(updatedAppointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailableTimeSlots(
            [FromQuery] Guid doctorId,
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime date)
        {
            try
            {
                if (doctorId == Guid.Empty)
                    return BadRequest("Doctor ID cannot be empty");
                
                // Generate time slots for the day
                var availableSlots = new List<DateTime>();
                var startTime = new DateTime(date.Year, date.Month, date.Day, 8, 0, 0); // Start at 8 AM
                var endTime = new DateTime(date.Year, date.Month, date.Day, 17, 0, 0);  // End at 5 PM
                
                // Standard 30-minute appointment duration
                var appointmentDuration = TimeSpan.FromMinutes(30);
                
                for (DateTime slot = startTime; slot <= endTime; slot = slot.AddMinutes(30))
                {
                    bool isAvailable = await _appointmentService.IsTimeSlotAvailableAsync(
                        doctorId, 
                        slot, 
                        appointmentDuration);
                    
                    if (isAvailable)
                    {
                        availableSlots.Add(slot);
                    }
                }
                
                return Ok(availableSlots);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slots for doctor {DoctorId} at clinic {ClinicId} on {Date}", 
                    doctorId, clinicId, date.ToShortDateString());
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
        public async Task<ActionResult<HealthcarePagedResultDTO<AppointmentDTO>>> GetAppointmentsByDoctor(
            Guid doctorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId, pageNumber, pageSize);
                return Ok(appointments);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<HealthcarePagedResultDTO<AppointmentDTO>>> GetAppointmentsByPet(
            Guid petId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPetAsync(petId, pageNumber, pageSize);
                return Ok(appointments);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for pet {PetId}", petId);
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

        [HttpPost("cancel/{id}")]
        public async Task<ActionResult<bool>> CancelAppointment(Guid id, [FromQuery] string reason)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
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

        [HttpGet]
        public async Task<ActionResult<HealthcarePagedResultDTO<AppointmentDTO>>> GetAppointments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync(pageNumber, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 

