//using Microsoft.AspNetCore.Mvc;
//using Core.Services;
//using Core.Services.Abstractions;
//using Shared.DoctorModels;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Presentation.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class DoctorController : ApiController
//    {
//        private readonly IServiceManager _serviceManager;

//        public DoctorController(IServiceManager serviceManager)
//        {
//            _serviceManager = serviceManager;
//        }

//        // GET: api/Doctor/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<DoctorDTO>> Get(int id)
//        {
//            var doctor = await _serviceManager.DoctorService.GetDoctorByIdAsync(id);
//            return Ok(doctor);
//        }

//        // GET: api/Doctor/specialty/{specialty}
//        [HttpGet("specialty/{specialty}")]
//        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetBySpecialty(string specialty)
//        {
//            var doctors = await _serviceManager.DoctorService.GetDoctorsBySpecialtyAsync(specialty);
//            return Ok(doctors);
//        }

//        // POST: api/Doctor
//        [HttpPost]
//        public async Task<ActionResult<DoctorDTO>> Create([FromBody] DoctorDTO model)
//        {
//            var createdDoctor = await _serviceManager.DoctorService.CreateDoctorAsync(model);
//            return CreatedAtAction(nameof(Get), new { id = createdDoctor.Id }, createdDoctor);
//        }

//        // PUT: api/Doctor/{id}
//        [HttpPut("{id}")]
//        public async Task<ActionResult<DoctorDTO>> Update(int id, [FromBody] DoctorDTO model)
//        {
//            var updatedDoctor = await _serviceManager.DoctorService.UpdateDoctorAsync(id, model);
//            return Ok(updatedDoctor);
//        }

//        // DELETE: api/Doctor/{id}
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            await _serviceManager.DoctorService.DeleteDoctorAsync(id);
//            return NoContent();
//        }

//        [HttpPost("{doctorId}/schedules")]
//        public async Task<IActionResult> AddSchedule(int doctorId, [FromBody] DoctorScheduleDTO dto)
//        {
//            await _serviceManager.DoctorScheduleService.AddScheduleAsync(doctorId, dto);
//            return NoContent();
//        }
//    }
//}

