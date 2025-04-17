using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitaryProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PrescriptionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Prescription/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDTO>> GetPrescription(Guid id)
        {
            var prescription = await _serviceManager.PrescriptionService.GetPrescriptionByIdAsync(id);
            
            if (prescription == null)
                return NotFound();
                
            return Ok(prescription);
        }

        // GET: api/Prescription/pet/{petProfileId}
        [HttpGet("pet/{petProfileId}")]
        public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetPrescriptionsByPetProfileId(Guid petProfileId)
        {
            var prescriptions = await _serviceManager.PrescriptionService.GetPrescriptionsByPetProfileIdAsync(petProfileId);
            return Ok(prescriptions);
        }

        // GET: api/Prescription/doctor/{doctorId}
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetPrescriptionsByDoctorId(Guid doctorId)
        {
            var prescriptions = await _serviceManager.PrescriptionService.GetPrescriptionsByDoctorIdAsync(doctorId);
            return Ok(prescriptions);
        }

        // GET: api/Prescription/active/pet/{petProfileId}
        [HttpGet("active/pet/{petProfileId}")]
        public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetActivePrescriptionsForPet(Guid petProfileId)
        {
            var prescriptions = await _serviceManager.PrescriptionService.GetActivePrescriptionsForPetAsync(petProfileId);
            return Ok(prescriptions);
        }

        // GET: api/Prescription/medicalrecord/{medicalRecordId}
        [HttpGet("medicalrecord/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetPrescriptionsByMedicalRecordId(Guid medicalRecordId)
        {
            var prescriptions = await _serviceManager.PrescriptionService.GetPrescriptionsByMedicalRecordIdAsync(medicalRecordId);
            return Ok(prescriptions);
        }

        // GET: api/Prescription/paged
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetPagedPrescriptions([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            if (pageIndex < 1)
                return BadRequest("Page index must be greater than or equal to 1.");
                
            if (pageSize < 1 || pageSize > 100)
                return BadRequest("Page size must be between 1 and 100.");
                
            var result = await _serviceManager.PrescriptionService.GetPrescriptionsByStatusAsync("All", pageIndex, pageSize);
            
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            
            return Ok(result.Items);
        }

        // POST: api/Prescription
        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<PrescriptionDTO>> CreatePrescription([FromBody] PrescriptionCreateDTO prescriptionDto)
        {
            try
            {
                var prescription = await _serviceManager.PrescriptionService.CreatePrescriptionAsync(prescriptionDto);
                return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Prescription/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<PrescriptionDTO>> UpdatePrescription(Guid id, [FromBody] PrescriptionUpdateDTO prescriptionDto)
        {
            try
            {
                var prescription = await _serviceManager.PrescriptionService.UpdatePrescriptionAsync(id, prescriptionDto);
                
                if (prescription == null)
                    return NotFound();
                    
                return Ok(prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Prescription/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult> DeletePrescription(Guid id)
        {
            await _serviceManager.PrescriptionService.DeletePrescriptionAsync(id);
            return NoContent();
        }
    }
} 