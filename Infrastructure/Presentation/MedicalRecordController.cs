//using Microsoft.AspNetCore.Mvc;
//using Services;
//using Services.Abstractions;
//using Shared.MedicalRecordModels;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Presentation.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class MedicalRecordController : ApiController
//    {
//        private readonly IServiceManager _serviceManager;

//        public MedicalRecordController(IServiceManager serviceManager)
//        {
//            _serviceManager = serviceManager;
//        }

//        // GET: api/MedicalRecord/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<MedicalRecordDTO>> Get(int id)
//        {
//            var record = await _serviceManager.MedicalRecordService.GetMedicalRecordByIdAsync(id);
//            return Ok(record);
//        }

//        // GET: api/MedicalRecord/pet/{petId}
//        [HttpGet("pet/{petId}")]
//        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetByPet(int petId)
//        {
//            var records = await _serviceManager.MedicalRecordService.GetRecordsByPetIdAsync(petId);
//            return Ok(records);
//        }

//        // POST: api/MedicalRecord
//        [HttpPost]
//        public async Task<ActionResult<MedicalRecordDTO>> Create([FromBody] MedicalRecordDTO model)
//        {
//            var createdRecord = await _serviceManager.MedicalRecordService.CreateMedicalRecordAsync(model);
//            return CreatedAtAction(nameof(Get), new { id = createdRecord.Id }, createdRecord);
//        }

//        // PUT: api/MedicalRecord/{id}
//        [HttpPut("{id}")]
//        public async Task<ActionResult<MedicalRecordDTO>> Update(int id, [FromBody] MedicalRecordDTO model)
//        {
//            var updatedRecord = await _serviceManager.MedicalRecordService.UpdateMedicalRecordAsync(id, model);
//            return Ok(updatedRecord);
//        }

//        // DELETE: api/MedicalRecord/{id}
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            await _serviceManager.MedicalRecordService.DeleteMedicalRecordAsync(id);
//            return NoContent();
//        }
//    }
//}
