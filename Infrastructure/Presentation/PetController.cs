using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Abstractions;
using Shared.PetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.PetEntities;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public PetController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Pet/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PetProfileDTO>> Get(int id)
        {
            var pet = await _serviceManager.PetService.GetPetByIdAsync(id);
            if (pet == null)
                return NotFound();
                
            // Map Pet to PetProfileDTO (you'd typically use an automapper here)
            var petDto = MapPetToDto(pet);
            return Ok(petDto);
        }

        // GET: api/Pet/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetByUser(string userId)
        {
            var pets = await _serviceManager.PetService.GetPetsByUserIdAsync(userId);
            
            // Map List<Pet> to List<PetProfileDTO>
            var petDtos = pets.Select(MapPetToDto);
            return Ok(petDtos);
        }

        // POST: api/Pet
        [HttpPost]
        public async Task<ActionResult<PetProfileDTO>> Create([FromBody] PetProfileDTO petDto)
        {
            // Map DTO to entity
            var pet = new Pet
            {
                PetName = petDto.PetName,
                BirthDate = petDto.BirthDate,
                Gender = petDto.Gender,
                Type = petDto.type,
                Color = petDto.Color,
                Avatar = petDto.Avatar,
                UserId = petDto.UserId
            };
            
            var createdPet = await _serviceManager.PetService.AddPetAsync(pet);
            
            // Map back to DTO
            var createdPetDto = MapPetToDto(createdPet);
            return CreatedAtAction(nameof(Get), new { id = createdPetDto.Id }, createdPetDto);
        }

        // PUT: api/Pet/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<PetProfileDTO>> Update(int id, [FromBody] PetProfileDTO petDto)
        {
            if (id != petDto.Id)
                return BadRequest("ID mismatch");
                
            // Map DTO to entity
            var pet = new Pet
            {
                Id = petDto.Id,
                PetName = petDto.PetName,
                BirthDate = petDto.BirthDate,
                Gender = petDto.Gender,
                Type = petDto.type,
                Color = petDto.Color,
                Avatar = petDto.Avatar,
                UserId = petDto.UserId
            };
            
            var success = await _serviceManager.PetService.UpdatePetAsync(pet);
            
            if (!success)
                return NotFound();
                
            return Ok(petDto);
        }

        // DELETE: api/Pet/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _serviceManager.PetService.DeletePetAsync(id);
            
            if (!success)
                return NotFound();
                
            return NoContent();
        }
        
        // Helper method to map Pet entity to PetProfileDTO
        private PetProfileDTO MapPetToDto(Pet pet)
        {
            return new PetProfileDTO
            {
                Id = pet.Id,
                PetName = pet.PetName,
                BirthDate = pet.BirthDate,
                Gender = pet.Gender,
                type = pet.Type,
                Color = pet.Color,
                Avatar = pet.Avatar,
                UserId = pet.UserId
            };
        }
    }
}
