using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities.SecurityEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public DoctorController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Doctor/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> Get(int id)
        {
            var doctor = await _serviceManager.DoctorService.GetDoctorByIdAsync(id);
            return Ok(doctor);
        }

        // GET: api/Doctor/specialty/{specialty}
        [HttpGet("specialty/{specialty}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetBySpecialty(string specialty)
        {
            var doctors = await _serviceManager.DoctorService.GetDoctorsBySpecialtyAsync(specialty);
            return Ok(doctors);
        }

        // GET: api/Doctor/user
        [HttpGet("GetDoctorProfile")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _serviceManager.DoctorService.GetDoctorByUserIdAsync(userId);
            
            if (doctor == null)
                return NotFound("Doctor profile not found. Please create a doctor profile first.");
                
            return Ok(doctor);
        }

        // POST: api/Doctor
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDTO>> Create([FromBody] DoctorDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                var createdDoctor = await _serviceManager.DoctorService.CreateDoctorAsync(model, userId);
                return CreatedAtAction(nameof(Get), new { id = createdDoctor.Id }, createdDoctor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Doctor/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<DoctorDTO>> Update(int id, [FromBody] DoctorDTO model)
        {
            // Check if user is admin or the doctor owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");
            
            // If not admin, verify ownership
            if (!isAdmin)
            {
                var doctor = await _serviceManager.DoctorService.GetDoctorByIdAsync(id);
                if (doctor.UserId != userId)
                {
                    return Forbid();
                }
            }
            
            var updatedDoctor = await _serviceManager.DoctorService.UpdateDoctorAsync(id, model);
            return Ok(updatedDoctor);
        }

        // DELETE: api/Doctor/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Delete(int id)
        {
            await _serviceManager.DoctorService.DeleteDoctorAsync(id);
            return NoContent();
        }

        // POST: api/Doctor/{doctorId}/schedules
        [HttpPost("{doctorId}/schedules")]
        [Authorize]
        public async Task<ActionResult<DoctorScheduleDTO>> AddSchedule(int doctorId, [FromBody] DoctorScheduleDTO schedule)
        {
            // Check if user is admin or the doctor owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");
            
            // If not admin, verify ownership
            if (!isAdmin)
            {
                var doctor = await _serviceManager.DoctorService.GetDoctorByIdAsync(doctorId);
                if (doctor.UserId != userId)
                {
                    return Forbid();
                }
            }
            
            try
            {
                // Set the doctor ID in the schedule data
                schedule = schedule with { DoctorId = doctorId };
                
                var result = await _serviceManager.DoctorScheduleService.AddScheduleAsync(doctorId, schedule);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest($"Invalid time format: {ex.Message}. Use HH:MM in 24-hour format.");
            }
        }
        
        // GET: api/Doctor/{doctorId}/schedules
        [HttpGet("{doctorId}/schedules")]
        public async Task<ActionResult<IEnumerable<DoctorScheduleDTO>>> GetSchedules(int doctorId)
        {
            try
            {
                var schedules = await _serviceManager.DoctorScheduleService.GetDoctorSchedulesAsync(doctorId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // DELETE: api/Doctor/schedules/{id}
        [HttpDelete("schedules/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteSchedule(int id)
        {
            try
            {
                await _serviceManager.DoctorScheduleService.DeleteScheduleAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
