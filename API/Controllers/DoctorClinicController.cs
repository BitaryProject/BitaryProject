using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor")]
    public class DoctorClinicController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;

        public DoctorClinicController(IDoctorService doctorService, IClinicService clinicService)
        {
            _doctorService = doctorService;
            _clinicService = clinicService;
        }

        [HttpGet("my-clinics")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetMyClinicAssociations()
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var clinics = await _doctorService.GetDoctorsByClinicIdAsync(doctor.Id);
                return Ok(clinics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-clinic")]
        public async Task<ActionResult<bool>> AddClinicAssociation(Guid clinicId)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var result = await _doctorService.AddClinicAssociationAsync(doctor.Id, clinicId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove-clinic/{clinicId}")]
        public async Task<ActionResult<bool>> RemoveClinicAssociation(Guid clinicId)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var result = await _doctorService.RemoveClinicAssociationAsync(doctor.Id, clinicId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-clinic")]
        public async Task<ActionResult<ClinicDTO>> CreateClinic(ClinicCreateDTO clinicDto)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                var clinic = await _clinicService.CreateClinicAsync(clinicDto, doctor.Id);
                return CreatedAtAction(nameof(GetMyClinicAssociations), clinic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-clinic/{clinicId}")]
        public async Task<ActionResult<ClinicDTO>> UpdateClinic(Guid clinicId, ClinicUpdateDTO clinicDto)
        {
            try
            {
                var userId = User.GetUserId();
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                
                if (doctor == null)
                    return NotFound("Doctor profile not found");
                
                // Verify doctor is associated with this clinic
                var isAssociated = await _clinicService.IsDoctorAssociatedWithClinicAsync(doctor.Id, clinicId);
                if (!isAssociated)
                    return Forbid("You are not associated with this clinic");
                
                var clinic = await _clinicService.UpdateClinicAsync(clinicId, clinicDto);
                return Ok(clinic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 