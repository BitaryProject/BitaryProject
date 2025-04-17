//using Microsoft.AspNetCore.Mvc;
//using Core.Services;
//using Core.Services.Abstractions;
//using Shared.AppointmentModels;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Presentation.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AppointmentController : ApiController
//    {
//        private readonly IServiceManager _serviceManager;

//        public AppointmentController(IServiceManager serviceManager)
//        {
//            _serviceManager = serviceManager;
//        }

//        // GET: api/Appointment/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<AppointmentDTO>> Get(int id)
//        {
//            var appointment = await _serviceManager.AppointmentService.GetAppointmentByIdAsync(id);
//            return Ok(appointment);
//        }

//        // GET: api/Appointment/user/{userId}
//        [HttpGet("user/{userId}")]
//        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByUser(string userId)
//        {
//            var appointments = await _serviceManager.AppointmentService.GetAppointmentsByUserIdAsync(userId);
//            return Ok(appointments);
//        }

//        // GET: api/Appointment/pet/{petId}
//        [HttpGet("pet/{petId}")]
//        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByPet(int petId)
//        {
//            var appointments = await _serviceManager.AppointmentService.GetAppointmentsByPetIdAsync(petId);
//            return Ok(appointments);
//        }

//        // POST: api/Appointment
//        [HttpPost]
//        public async Task<ActionResult<AppointmentDTO>> Create([FromBody] AppointmentDTO model)
//        {
//            var createdAppointment = await _serviceManager.AppointmentService.CreateAppointmentAsync(model);
//            return CreatedAtAction(nameof(Get), new { id = createdAppointment.Id }, createdAppointment);
//        }

//        // PUT: api/Appointment/{id}
//        [HttpPut("{id}")]
//        public async Task<ActionResult<AppointmentDTO>> Update(int id, [FromBody] AppointmentDTO model)
//        {
//            var updatedAppointment = await _serviceManager.AppointmentService.UpdateAppointmentAsync(id, model);
//            return Ok(updatedAppointment);
//        }

//        // DELETE: api/Appointment/{id}
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            await _serviceManager.AppointmentService.DeleteAppointmentAsync(id);
//            return NoContent();
//        }



//    }
//}

