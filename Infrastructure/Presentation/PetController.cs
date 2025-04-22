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
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PetProfileDTO>> Get(int id)
        {
            try
            {
                // Get the current user ID from the token
                var currentUserId = GetCurrentUserId();
                
                var pet = await _serviceManager.PetService.GetPetByIdAsync(id);
                if (pet == null)
                    return NotFound();
                
                // Verify that the user owns this pet
                if (pet.UserId != currentUserId)
                    return StatusCode(403, new { message = "You don't have permission to view this pet" });
                    
                // Map Pet to PetProfileDTO (you'd typically use an automapper here)
                var petDto = MapPetToDto(pet);
                return Ok(petDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You must be logged in to view pets" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PetProfileDTO>> Create([FromBody] UpdatePetRequestDTO createDto)
        {
            try
            {
                // Get the current user ID from the token
                var currentUserId = GetCurrentUserId();
                
                // Map DTO to entity
                var pet = new Pet
                {
                    PetName = createDto.PetName,
                    BirthDate = createDto.BirthDate,
                    Gender = createDto.Gender,
                    PetType = createDto.type,
                    Color = createDto.Color,
                    Avatar = createDto.Avatar,
                    UserId = currentUserId // Always use the authenticated user's ID
                };
                
                var createdPet = await _serviceManager.PetService.AddPetAsync(pet);
                
                // Map back to DTO
                var createdPetDto = MapPetToDto(createdPet);
                return CreatedAtAction(nameof(Get), new { id = createdPetDto.Id }, createdPetDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You must be logged in to create pets" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        // PUT: api/Pet/{id}
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<PetProfileDTO>> Update(int id, [FromBody] UpdatePetRequestDTO updateDto)
        {
            try
            {
                // Get the current user ID from the token
                var currentUserId = GetCurrentUserId();
                
                // First check if the pet exists
                var existingPet = await _serviceManager.PetService.GetPetByIdAsync(id);
                if (existingPet == null)
                    return NotFound();
                
                // Verify that the user owns this pet
                if (existingPet.UserId != currentUserId)
                    return StatusCode(403, new { message = "You don't have permission to update this pet" });
                
                // Map DTO to entity, preserving the ID and UserID from the existing pet
                var pet = new Pet
                {
                    Id = id, // Use the ID from the URL
                    PetName = updateDto.PetName,
                    BirthDate = updateDto.BirthDate,
                    Gender = updateDto.Gender,
                    PetType = updateDto.type,
                    Color = updateDto.Color,
                    Avatar = updateDto.Avatar,
                    UserId = currentUserId // Ensure we're using the authenticated user's ID
                };
                
                var success = await _serviceManager.PetService.UpdatePetAsync(pet);
                
                if (!success)
                    return StatusCode(500, new { message = "Failed to update pet" });
                
                // Map the updated pet to DTO for response
                var updatedPetDto = MapPetToDto(pet);
                return Ok(updatedPetDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You must be logged in to update pets" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        // DELETE: api/Pet/{id}
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Get the current user ID from the token
                var currentUserId = GetCurrentUserId();
                
                // First check if the pet exists
                var existingPet = await _serviceManager.PetService.GetPetByIdAsync(id);
                if (existingPet == null)
                    return NotFound();
                
                // Verify that the user owns this pet
                if (existingPet.UserId != currentUserId)
                    return StatusCode(403, new { message = "You don't have permission to delete this pet" });
                
                var success = await _serviceManager.PetService.DeletePetAsync(id);
                
                if (!success)
                    return StatusCode(500, new { message = "Failed to delete pet" });
                
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "You must be logged in to delete pets" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
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
