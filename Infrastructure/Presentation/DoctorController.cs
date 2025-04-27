using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities.SecurityEntities;
using Domain.Entities.DoctorEntites;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly UserManager<User> _userManager;

        public DoctorController(
            IServiceManager serviceManager,
            UserManager<User> userManager)
        {
            _serviceManager = serviceManager;
            _userManager = userManager;
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
        public async Task<ActionResult<DoctorDTO>> Create([FromBody] DoctorCreateModel createModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Debug information about claims
            Console.WriteLine($"====== DEBUG: USER CREATE DOCTOR ATTEMPT ======");
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Claims in token:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"  {claim.Type}: {claim.Value}");
            }

            // Check roles using multiple methods
            Console.WriteLine($"IsInRole('Admin'): {User.IsInRole("Admin")}");
            Console.WriteLine($"IsInRole('Doctor'): {User.IsInRole("Doctor")}");
            Console.WriteLine($"HasClaim(Role, Admin): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin")}");
            Console.WriteLine($"HasClaim(Role, Doctor): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Doctor")}");
            Console.WriteLine($"HasClaim(Role, 3): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "3")}");
            Console.WriteLine($"HasClaim(Role, 2): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "2")}");
            Console.WriteLine($"IsUserInRole(Role.Admin): {IsUserInRole(Role.Admin)}");
            Console.WriteLine($"IsUserInRole(Role.Doctor): {IsUserInRole(Role.Doctor)}");

            // Manual authorization check - if neither Doctor nor Admin, return 403 Forbidden
            bool isDoctor = IsUserInRole(Role.Doctor);
            bool isAdmin = IsUserInRole(Role.Admin);

            if (!isDoctor && !isAdmin)
            {
                Console.WriteLine("ACCESS DENIED: User is neither doctor nor admin");
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest("User not found. Please log in again.");

            // Display UserRole from database
            Console.WriteLine($"User.UserRole: {user.UserRole} (Value: {(int)user.UserRole})");

            try
            {
                // Create a full doctor DTO with user information from token
                var doctorDto = new DoctorDTO
                {
                    // User only needs to provide these fields
                    Specialty = createModel.Specialty,
                    ClinicId = createModel.ClinicId,

                    // Auto-populate these fields from the user's account
                    Name = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Gender = MapUserGenderToDoctorGender(user.Gender),
                    UserId = userId
                };

                var createdDoctor = await _serviceManager.DoctorService.CreateDoctorAsync(doctorDto, userId);
                return CreatedAtAction(nameof(Get), new { id = createdDoctor.Id }, createdDoctor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Helper method to map Gender to DocGender
        private DocGender MapUserGenderToDoctorGender(Gender userGender)
        {
            return userGender == Gender.male || userGender == Gender.m
                ? DocGender.male
                : DocGender.female;
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
        public async Task<ActionResult> Delete(int id)
        {
            // Debug information about claims
            Console.WriteLine($"====== DEBUG: USER DELETE DOCTOR ATTEMPT ======");

            // Manual authorization check - if neither Doctor nor Admin, return 403 Forbidden
            bool isDoctor = IsUserInRole(Role.Doctor);
            bool isAdmin = IsUserInRole(Role.Admin);

            if (!isDoctor && !isAdmin)
            {
                Console.WriteLine("ACCESS DENIED: User is neither doctor nor admin");
                return Forbid();
            }

            // Check if user is the doctor owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _serviceManager.DoctorService.GetDoctorByIdAsync(id);

            // If not admin, verify ownership
            if (!isAdmin && doctor.UserId != userId)
            {
                Console.WriteLine("ACCESS DENIED: User is not the owner of this doctor profile");
                return Forbid();
            }

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
                
                // Format the response as requested
                return Ok(new {
                    id = result.Id,
                    doctorId = result.DoctorId,
                    time = result.ScheduleDate,
                    startTimeString = result.StartTimeString,
                    endTimeString = result.EndTimeString,
                    doctorName = result.DoctorName
                });
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
        [Authorize(Roles = "Doctor,Admin")]
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

        // Helper method to check if a user is in a specific role,
        // checking both ASP.NET Identity roles and Role enum claims
        private bool IsUserInRole(Role role)
        {
            string roleName = role.ToString();
            int roleValue = (int)role;

            return User.IsInRole(roleName) ||
                   User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == roleName) ||
                   User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == roleValue.ToString());
        }
    }
}