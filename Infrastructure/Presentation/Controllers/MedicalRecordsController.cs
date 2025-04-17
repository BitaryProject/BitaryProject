/*
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
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPetProfileService _petProfileService;
        private readonly IPetOwnerService _petOwnerService;
        private readonly IDoctorService _doctorService;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(
            IMedicalRecordService medicalRecordService,
            IPetProfileService petProfileService,
            IPetOwnerService petOwnerService,
            IDoctorService doctorService,
            ILogger<MedicalRecordsController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _petProfileService = petProfileService;
            _petOwnerService = petOwnerService;
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecordDTO>> GetMedicalRecord(Guid id)
        {
            try
            {
                var medicalRecord = await _medicalRecordService.GetByIdAsync(id);
                
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(petOwner.Id);
                    bool isOwnersRecord = false;
                    
                    foreach (var pet in petProfiles)
                    {
                        if (pet.Id == medicalRecord.PetId)
                        {
                            isOwnersRecord = true;
                            break;
                        }
                    }
                    
                    if (!isOwnersRecord)
                        return Forbid("You are not authorized to view this medical record");
                }
                
                return Ok(medicalRecord);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical record {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetMedicalRecordsByPet(
            Guid petId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    var pet = await _petProfileService.GetPetProfileByIdAsync(petId);
                    if (pet == null || pet.OwnerId != petOwner.Id)
                        return Forbid("You can only view medical records for your own pets");
                }
                
                var records = await _medicalRecordService.GetMedicalRecordsByPetAsync(petId, pageIndex, pageSize);
                return Ok(ConvertToPagedResult(records));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records for pet {PetId}", petId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetMedicalRecordsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDateRangeAsync(startDate, endDate, pageIndex, pageSize);
                
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    var petIds = await _petProfileService.GetPetIdsByOwnerIdAsync(petOwner.Id);
                    var filteredRecords = new List<MedicalRecordDTO>();
                    
                    foreach (var record in records.Items)
                    {
                        if (petIds.Contains(record.PetId))
                        {
                            filteredRecords.Add(record);
                        }
                    }
                    
                    return Ok(new PagedResultDTO<MedicalRecordDTO>
                    {
                        PageIndex = records.PageIndex,
                        PageSize = records.PageSize,
                        TotalCount = filteredRecords.Count,
                        Items = filteredRecords
                    });
                }
                
                return Ok(ConvertToPagedResult(records));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records by date range");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("diagnosis")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetMedicalRecordsByDiagnosis(
            [FromQuery] string diagnosis, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDiagnosisAsync(diagnosis, pageIndex, pageSize);
                
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    var petIds = await _petProfileService.GetPetIdsByOwnerIdAsync(petOwner.Id);
                    var filteredRecords = new List<MedicalRecordDTO>();
                    
                    foreach (var record in records.Items)
                    {
                        if (petIds.Contains(record.PetId))
                        {
                            filteredRecords.Add(record);
                        }
                    }
                    
                    return Ok(new PagedResultDTO<MedicalRecordDTO>
                    {
                        PageIndex = records.PageIndex,
                        PageSize = records.PageSize,
                        TotalCount = filteredRecords.Count,
                        Items = filteredRecords
                    });
                }
                
                return Ok(ConvertToPagedResult(records));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records by diagnosis");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

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
                
                var medicalRecord = await _medicalRecordService.CreateMedicalRecordAsync(recordDto);
                return CreatedAtAction(nameof(GetMedicalRecord), new { id = medicalRecord.Id }, medicalRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical record");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

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
                
                var existingRecord = await _medicalRecordService.GetByIdAsync(id);
                
                if (existingRecord == null)
                    return NotFound($"Medical record with ID {id} not found");
                
                if (existingRecord.DoctorId != doctor.Id)
                    return Forbid("You can only update medical records that you created");
                
                var medicalRecord = await _medicalRecordService.UpdateMedicalRecordAsync(id, recordDto);
                return Ok(medicalRecord);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical record {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> DeleteMedicalRecord(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var existingRecord = await _medicalRecordService.GetByIdAsync(id);
                
                if (existingRecord == null)
                    return NotFound($"Medical record with ID {id} not found");
                
                if (existingRecord.DoctorId != doctor.Id)
                    return Forbid("You can only delete medical records that you created");
                
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

        // Add helper method to consistently convert between DTO types
        private PagedResultDTO<T> ConvertToPagedResult<T>(HealthcarePagedResultDTO<T> healthcareResult)
        {
            if (healthcareResult == null)
                return new PagedResultDTO<T>();
                
            return new PagedResultDTO<T>
            {
                Items = healthcareResult.Items,
                TotalCount = healthcareResult.TotalCount,
                PageIndex = healthcareResult.PageIndex,
                PageSize = healthcareResult.PageSize
            };
        }
    }
} 


*/
