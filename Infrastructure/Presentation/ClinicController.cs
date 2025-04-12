/*using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Abstractions;
using Shared.ClinicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public ClinicController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Clinic/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> Get(Guid id)
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
        public async Task<ActionResult<ClinicDTO>> Create([FromBody] ClinicDTO model)
        {
            var createdClinic = await _serviceManager.ClinicService.CreateClinicAsync(model);
            return CreatedAtAction(nameof(Get), new { id = createdClinic.Id }, createdClinic);
        }

        // PUT: api/Clinic/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ClinicDTO>> Update(Guid id, [FromBody] ClinicDTO model)
        {
            var updatedClinic = await _serviceManager.ClinicService.UpdateClinicAsync(id, model);
            return Ok(updatedClinic);
        }

        // DELETE: api/Clinic/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _serviceManager.ClinicService.DeleteClinicAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchClinics(
    [FromQuery] string city,
    [FromQuery] int radius = 10)
        {
            var clinics = await _serviceManager.ClinicSearchService.SearchClinicsAsync(city, radius);
            return Ok(clinics);
        }
    }
}
*/