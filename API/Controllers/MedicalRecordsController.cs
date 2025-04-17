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
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorService _doctorService;
        private readonly IPetProfileService _petProfileService;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(
            IMedicalRecordService medicalRecordService,
            IDoctorService doctorService,
            IPetProfileService petProfileService,
            ILogger<MedicalRecordsController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _petProfileService = petProfileService;
            _logger = logger;
        }

        // Endpoint accessible to both doctors and pet owners
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,PetOwner")]
        public async Task<ActionResult<MedicalRecordDTO>> GetMedicalRecord(Guid id)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Pet owners can only view their own pets' medical records
        [HttpGet("pet/{petId}")]
        [Authorize(Roles = "Doctor,PetOwner")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetMedicalRecordsByPet(Guid petId)
        {
            try
            {
                // If user is a pet owner, verify they own this pet
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petProfile = await _petProfileService.GetPetProfileByIdAsync(petId);
                    
                    // Check if the pet belongs to the current user
                    if (petProfile == null || petProfile.OwnerId.ToString() != userId)
                        return Forbid("You do not have permission to view this pet's medical records");
                }
                
                var records = await _medicalRecordService.GetMedicalRecordsByPetIdAsync(petId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Only doctors can create medical records
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> CreateMedicalRecord(MedicalRecordCreateDTO recordDto)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                // Set the doctor ID if not provided
                if (recordDto.DoctorId == Guid.Empty)
                    recordDto.DoctorId = doctor.Id;
                
                // Verify that the doctor is the one creating the record
                if (recordDto.DoctorId != doctor.Id)
                    return Forbid("You can only create medical records as yourself");
                
                var record = await _medicalRecordService.CreateMedicalRecordAsync(recordDto);
                return CreatedAtAction(nameof(GetMedicalRecord), new { id = record.Id }, record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Only doctors can update medical records
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> UpdateMedicalRecord(Guid id, MedicalRecordUpdateDTO recordDto)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                // Get the existing record to check if this doctor created it
                var existingRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
                if (existingRecord == null)
                    return NotFound($"Medical record with ID {id} not found");
                
                // Verify the doctor is the one who created the record
                if (existingRecord.DoctorId != doctor.Id)
                    return Forbid("You can only update medical records that you created");
                
                var updatedRecord = await _medicalRecordService.UpdateMedicalRecordAsync(id, recordDto);
                return Ok(updatedRecord);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Doctor's medical records - for a doctor to see all records they've created
        [HttpGet("my-records")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetDoctorMedicalRecords()
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var records = await _medicalRecordService.GetMedicalRecordsByDoctorIdAsync(doctor.Id);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Pet owner's pets' medical records
        [HttpGet("my-pets-records")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetOwnerPetsMedicalRecords()
        {
            try
            {
                var userId = User.GetUserId();
                var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(Guid.Parse(userId));
                
                List<MedicalRecordDTO> allRecords = new List<MedicalRecordDTO>();
                foreach (var pet in petProfiles)
                {
                    var petRecords = await _medicalRecordService.GetMedicalRecordsByPetIdAsync(pet.Id);
                    allRecords.AddRange(petRecords);
                }
                
                return Ok(allRecords);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDateRangeAsync(startDate, endDate, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical records between {StartDate} and {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("diagnosis")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetByDiagnosis(
            [FromQuery] string diagnosis, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDiagnosisAsync(diagnosis, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical records with diagnosis {Diagnosis}", diagnosis);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _medicalRecordService.DeleteMedicalRecordAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 