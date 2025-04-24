global using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.MedicalRecordModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.SecurityEntities;
using System.Security.Claims;
using Domain.Entities.AppointmentEntities;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public MedicalRecordController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/MedicalRecord/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Admin,PetOwner")]
        public async Task<ActionResult<MedicalRecordDTO>> GetMedicalRecord(int id)
        {
            var record = await _serviceManager.MedicalRecordService.GetMedicalRecordByIdAsync(id);
            
            // Check access permissions
            bool isAdmin = User.IsInRole(Role.Admin.ToString());
            bool isDoctor = User.IsInRole(Role.Doctor.ToString());
            
            if (!isAdmin && !isDoctor)
            {
                // For pet owners, check if they own the pet
                var pet = await _serviceManager.PetService.GetPetByIdAsync(record.PetId);
                if (pet.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }
            }
            else if (isDoctor)
            {
                // Doctors can only view records they created or for appointments assigned to them
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                if (!string.IsNullOrEmpty(doctorIdClaim) && record.DoctorId.ToString() != doctorIdClaim)
                {
                    return Forbid();
                }
            }
            
            return Ok(record);
        }

        // GET: api/MedicalRecord/pet/{petId}
        [HttpGet("pet/{petId}")]
        [Authorize(Roles = "Doctor,Admin,PetOwner")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetMedicalRecordsByPet(int petId)
        {
            // Check access permissions
            bool isAdmin = User.IsInRole(Role.Admin.ToString());
            bool isDoctor = User.IsInRole(Role.Doctor.ToString());
            
            if (!isAdmin && !isDoctor)
            {
                // For pet owners, check if they own the pet
                var pet = await _serviceManager.PetService.GetPetByIdAsync(petId);
                if (pet.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }
            }
            
            var records = await _serviceManager.MedicalRecordService.GetRecordsByPetIdAsync(petId);
            return Ok(records);
        }
        // POST: api/MedicalRecord/{appointmentId}
        [HttpPost("{appointmentId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> CreateMedicalRecord(int appointmentId, [FromBody] MedicalRecordCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the doctor ID from the claims
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                if (string.IsNullOrEmpty(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int doctorId))
                {
                    return BadRequest("Doctor ID not found in your token. Please ensure your doctor profile is created.");
                }

                // Verify the appointment exists and is for this doctor
                var appointment = await _serviceManager.AppointmentService.GetAppointmentByIdAsync(appointmentId);
                if (appointment.DoctorId != doctorId)
                {
                    return BadRequest("You can only create medical records for your own appointments.");
                }

                // Check if the appointment status is Completed or can be marked as Completed
                if (appointment.Status != AppointmentStatus.Approved &&
                    appointment.Status != AppointmentStatus.Completed)
                {
                    return BadRequest("You can only create medical records for approved or completed appointments.");
                }
                var createdRecord = await _serviceManager.MedicalRecordService.CreateMedicalRecordForAppointmentAsync(
                    appointmentId, model, doctorId);

                return CreatedAtAction(nameof(GetMedicalRecord), new { id = createdRecord.Id }, createdRecord);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET: api/MedicalRecord/doctor/{doctorId}
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetMedicalRecordsByDoctor(int doctorId)
        {
            // Check access permissions
            bool isAdmin = User.IsInRole(Role.Admin.ToString());
            bool isDoctor = User.IsInRole(Role.Doctor.ToString());
            
            if (isDoctor)
            {
                // Doctors can only view their own records
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                if (!string.IsNullOrEmpty(doctorIdClaim) && doctorId.ToString() != doctorIdClaim)
                {
                    return Forbid();
                }
            }
            
            var records = await _serviceManager.MedicalRecordService.GetRecordsByDoctorIdAsync(doctorId);
            return Ok(records);
        }
        
        // GET: api/MedicalRecord/appointment/{appointmentId}
        [HttpGet("appointment/{appointmentId}")]
        [Authorize(Roles = "Doctor,Admin,PetOwner")]
        public async Task<ActionResult<MedicalRecordDTO>> GetMedicalRecordByAppointment(int appointmentId)
        {
            try
            {
                // Check permissions first
                var appointment = await _serviceManager.AppointmentService.GetAppointmentByIdAsync(appointmentId);
                
                bool isAdmin = User.IsInRole(Role.Admin.ToString());
                bool isDoctor = User.IsInRole(Role.Doctor.ToString());
                
                if (!isAdmin && !isDoctor)
                {
                    // For pet owners, check if they own the appointment
                    if (appointment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                    {
                        return Forbid();
                    }
                }
                else if (isDoctor)
                {
                    // Doctors can only view records for appointments assigned to them
                    var doctorIdClaim = User.FindFirstValue("DoctorId");
                    if (!string.IsNullOrEmpty(doctorIdClaim) && appointment.DoctorId.ToString() != doctorIdClaim)
                    {
                        return Forbid();
                    }
                }
                
                var record = await _serviceManager.MedicalRecordService.GetRecordByAppointmentIdAsync(appointmentId);
                return Ok(record);
            }
            catch (MedicalRecordNotFoundException)
            {
                return NotFound($"No medical record found for appointment ID: {appointmentId}");
            }
        }

        // POST: api/MedicalRecord
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> CreateMedicalRecord([FromBody] MedicalRecordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Verify the doctor is creating a record for their own appointment
            var doctorIdClaim = User.FindFirstValue("DoctorId");
            if (!string.IsNullOrEmpty(doctorIdClaim) && model.DoctorId.ToString() != doctorIdClaim)
            {
                return BadRequest("You can only create medical records for your own appointments.");
            }
            
            // Verify the appointment exists and is valid
            try
            {
                var appointment = await _serviceManager.AppointmentService.GetAppointmentByIdAsync(model.AppointmentId);
                
                // Check if the appointment belongs to the doctor and the correct pet
                if (appointment.DoctorId != model.DoctorId || appointment.PetId != model.PetId)
                {
                    return BadRequest("The appointment details do not match the provided doctor and pet IDs.");
                }
            }
            catch (AppointmentNotFoundException)
            {
                return BadRequest($"Appointment with ID {model.AppointmentId} not found.");
            }
            
            try
            {
                var createdRecord = await _serviceManager.MedicalRecordService.CreateMedicalRecordAsync(model);
                return CreatedAtAction(nameof(GetMedicalRecord), new { id = createdRecord.Id }, createdRecord);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/MedicalRecord/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> UpdateMedicalRecord(int id, [FromBody] MedicalRecordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                // Check if the record exists
                var existingRecord = await _serviceManager.MedicalRecordService.GetMedicalRecordByIdAsync(id);
                
                // Verify the doctor is updating their own record
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                if (!string.IsNullOrEmpty(doctorIdClaim) && existingRecord.DoctorId.ToString() != doctorIdClaim)
                {
                    return Forbid();
                }
                
                var updatedRecord = await _serviceManager.MedicalRecordService.UpdateMedicalRecordAsync(id, model);
                return Ok(updatedRecord);
            }
            catch (MedicalRecordNotFoundException)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/MedicalRecord/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult> DeleteMedicalRecord(int id)
        {
            try
            {
                // Check if the record exists
                var existingRecord = await _serviceManager.MedicalRecordService.GetMedicalRecordByIdAsync(id);
                
                // Check permissions
                bool isAdmin = User.IsInRole(Role.Admin.ToString());
                
                if (!isAdmin)
                {
                    // Verify the doctor is deleting their own record
                    var doctorIdClaim = User.FindFirstValue("DoctorId");
                    if (!string.IsNullOrEmpty(doctorIdClaim) && existingRecord.DoctorId.ToString() != doctorIdClaim)
                    {
                        return Forbid();
                    }
                }
                
                await _serviceManager.MedicalRecordService.DeleteMedicalRecordAsync(id);
                return NoContent();
            }
            catch (MedicalRecordNotFoundException)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
