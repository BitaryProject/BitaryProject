using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Abstractions;
using Shared.PetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.PetEntities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Changed route to avoid conflict
    public class PetController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public PetController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // Helper method to get current user ID from claims
        private string GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // If user ID is not available, try to get the email
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue(ClaimTypes.Email);
                
                // If email is also not available
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User information not found in token");
                }
            }
            
            return userId;
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

        // GET: api/Pet/user
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetUserPets()
        {
            try
            {
                // Get the current user's ID from the token
                var userId = GetCurrentUserId();
                
                // Get all pets for the authenticated user
                var pets = await _serviceManager.PetService.GetPetsByUserIdAsync(userId);
                
                // Map List<Pet> to List<PetProfileDTO>
                var petDtos = pets.Select(MapPetToDto);
                return Ok(petDtos);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You must be logged in to view your pets" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
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
                PetType = petDto.type,
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
                PetType = petDto.type,
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
                type = pet.PetType,
                Color = pet.Color,
                Avatar = pet.Avatar,
                UserId = pet.UserId
            };
        }
    }
}
