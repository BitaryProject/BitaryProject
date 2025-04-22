using Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ClinicModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities.SecurityEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ClinicController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Clinic/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> Get(int id)
        {
            var clinic = await _serviceManager.ClinicService.GetClinicByIdAsync(id);
            return Ok(clinic);
        }

        // GET: api/Clinic
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetAll()
        {
            var clinics = await _serviceManager.ClinicService.GetAllClinicsAsync();
            return Ok(clinics);
        }

        // POST: api/Clinic
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ClinicDTO>> Create([FromBody] ClinicRequestDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdClinic = await _serviceManager.ClinicService.CreateClinicAsync(model, userId);
            return CreatedAtAction(nameof(Get), new { id = createdClinic.Id }, createdClinic);
        }

        // PUT: api/Clinic/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ClinicDTO>> Update(int id, [FromBody] ClinicUpdateDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Debug information about claims
            Console.WriteLine($"====== DEBUG: USER UPDATE CLINIC ATTEMPT ======");
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Clinic ID: {id}");
            Console.WriteLine($"Claims in token:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"  {claim.Type}: {claim.Value}");
            }

            Console.WriteLine($"IsInRole('Admin'): {User.IsInRole("Admin")}");
            Console.WriteLine($"IsInRole('Doctor'): {User.IsInRole("Doctor")}");
            Console.WriteLine($"HasClaim(Role, Admin): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin")}");
            Console.WriteLine($"HasClaim(Role, Doctor): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Doctor")}");
            Console.WriteLine($"HasClaim(Role, 3): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "3")}");
            Console.WriteLine($"HasClaim(Role, 2): {User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "2")}");
            Console.WriteLine($"IsUserInRole(Role.Admin): {IsUserInRole(Role.Admin)}");
            Console.WriteLine($"IsUserInRole(Role.Doctor): {IsUserInRole(Role.Doctor)}");
            
            // Check if user is admin or doctor first - completely manual role check
            bool isAdmin = IsUserInRole(Role.Admin);
            bool isDoctor = IsUserInRole(Role.Doctor);
            
            Console.WriteLine($"isAdmin: {isAdmin}");
            Console.WriteLine($"isDoctor: {isDoctor}");
            
            // If neither admin nor doctor, forbid access
            if (!isAdmin && !isDoctor)
            {
                Console.WriteLine("ACCESS DENIED: User is neither admin nor doctor");
                return Forbid();
            }

            // If not admin, verify ownership
            if (!isAdmin)
            {
                var clinics = await _serviceManager.ClinicService.GetClinicsByOwnerIdAsync(userId);
                bool isOwner = clinics.Any(c => c.Id == id);
                Console.WriteLine($"isOwner: {isOwner}");
                
                if (!isOwner)
                {
                    Console.WriteLine("ACCESS DENIED: User is not the owner of this clinic");
                    return Forbid();
                }
                
                Console.WriteLine("ACCESS GRANTED: User is doctor and owner");
                var updatedClinic = await _serviceManager.ClinicService.UpdateClinicBasicInfoAsync(id, model);
                return Ok(updatedClinic);
            }
            
            // For admins, use the same update method
            Console.WriteLine("ACCESS GRANTED: User is admin");
            var result = await _serviceManager.ClinicService.UpdateClinicBasicInfoAsync(id, model);
            return Ok(result);
        }

        // DELETE: api/Clinic/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceManager.ClinicService.DeleteClinicAsync(id);
            return NoContent();
        }

        // GET: api/Clinic/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetPending()
        {
            var pendingClinics = await _serviceManager.ClinicService.GetPendingClinicsAsync();
            return Ok(pendingClinics);
        }

        // PUT: api/Clinic/{id}/approve
        [HttpPut("{id}/admin/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClinicDTO>> Approve(int id)
        {
            var approvedClinic = await _serviceManager.ClinicService.ApproveClinicAsync(id);
            return Ok(approvedClinic);
        }

        // PUT: api/Clinic/{id}/reject
        [HttpPut("{id}/admin/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClinicDTO>> Reject(int id)
        {
            var rejectedClinic = await _serviceManager.ClinicService.RejectClinicAsync(id);
            return Ok(rejectedClinic);
        }

        // GET: api/Clinic/my
        [HttpGet("my")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetMyClinics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clinics = await _serviceManager.ClinicService.GetClinicsByOwnerIdAsync(userId);
            return Ok(clinics);
        }

        // POST: api/Clinic/{clinicId}/doctors/{doctorId}
        [HttpPost("{clinicId}/doctors/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<ClinicDTO>> AddDoctorToClinic(int clinicId, int doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Debug information about claims
            Console.WriteLine($"====== DEBUG: USER ADD DOCTOR TO CLINIC ATTEMPT ======");
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Clinic ID: {clinicId}, Doctor ID: {doctorId}");
            
            // Check if user is admin or doctor first
            bool isAdmin = IsUserInRole(Role.Admin);
            bool isDoctor = IsUserInRole(Role.Doctor);
            
            Console.WriteLine($"isAdmin: {isAdmin}");
            Console.WriteLine($"isDoctor: {isDoctor}");
            
            // If neither admin nor doctor, forbid access
            if (!isAdmin && !isDoctor)
            {
                Console.WriteLine("ACCESS DENIED: User is neither admin nor doctor");
                return Forbid();
            }
            
            // If not admin, verify ownership
            if (!isAdmin)
            {
                var clinics = await _serviceManager.ClinicService.GetClinicsByOwnerIdAsync(userId);
                bool isOwner = clinics.Any(c => c.Id == clinicId);
                Console.WriteLine($"isOwner: {isOwner}");
                
                if (!isOwner)
                {
                    Console.WriteLine("ACCESS DENIED: User is not the owner of this clinic");
                    return Forbid();
                }
            }
            
            Console.WriteLine("ACCESS GRANTED: User authorized to add doctor to clinic");
            var clinic = await _serviceManager.ClinicService.AddDoctorToClinicAsync(clinicId, doctorId);
            return Ok(clinic);
        }

        // PUT: api/Clinic/{id}/status
        [HttpPut("{id}/admin/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClinicDTO>> UpdateStatus(int id, [FromBody] ClinicStatusUpdateDTO statusUpdate)
        {
            var updatedClinic = await _serviceManager.ClinicService.UpdateClinicStatusAsync(id, statusUpdate);
            return Ok(updatedClinic);
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
