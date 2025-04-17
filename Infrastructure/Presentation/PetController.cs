//using Microsoft.AspNetCore.Mvc;
//using Core.Services;
//using Core.Services.Abstractions;
//using Shared.PetModels;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Presentation.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class PetController : ApiController
//    {
//        private readonly IServiceManager _serviceManager;

//        public PetController(IServiceManager serviceManager)
//        {
//            _serviceManager = serviceManager;
//        }

//        // GET: api/Pet/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<PetProfileDTO>> Get(string id)
//        {
//            var pet = await _serviceManager.PetService.GetPetAsync(id);
//            return Ok(pet);
//        }

//        // GET: api/Pet/user/{userId}
//        [HttpGet("user/{userId}")]
//        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetByUser(string userId)
//        {
//            var pets = await _serviceManager.PetService.GetPetsByUserIdAsync(userId);
//            return Ok(pets);
//        }

//        // POST: api/Pet
//        [HttpPost]
//        public async Task<ActionResult<PetProfileDTO>> Create([FromBody] PetProfileDTO petDto)
//        {
//            var createdPet = await _serviceManager.PetService.CreatePetAsync(petDto);
//            return CreatedAtAction(nameof(Get), new { id = createdPet.Id }, createdPet);
//        }

//        // PUT: api/Pet/{id}
//        [HttpPut("{id}")]
//        public async Task<ActionResult<PetProfileDTO>> Update(string id, [FromBody] PetProfileDTO petDto)
//        {
//            var updatedPet = await _serviceManager.PetService.UpdatePetAsync(id, petDto);
//            return Ok(updatedPet);
//        }

//        // DELETE: api/Pet/{id}
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(string id)
//        {
//            await _serviceManager.PetService.DeletePetAsync(id);
//            return NoContent();
//        }
//    }
//}

