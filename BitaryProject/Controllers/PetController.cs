// COMMENTED OUT TO USE THE CONTROLLER IN Infrastructure/Presentation/PetController.cs
// This controller is commented out to avoid conflict with the PetController in the Presentation layer.
// The Presentation layer controller is now using the route "api/presentation/Pet" to avoid conflicts.
// If you need the features from this controller (authentication, etc.), consider adding them to the 
// Presentation controller instead.

/*
using AutoMapper;
using Domain.Entities.PetEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.PetModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BitaryProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public PetController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            // First try to get the user ID directly
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // If user ID is not available, get the email and use it as ID
            if (string.IsNullOrEmpty(userId))
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    throw new UnauthorizedAccessException("User information not found in token");
                }
                
                // Use the email as the user ID
                userId = email;
            }
            
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetUserPets()
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                var pets = await _serviceManager.PetService.GetPetsByUserIdAsync(userId);
                return Ok(_mapper.Map<IEnumerable<PetDTO>>(pets));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetPet(int id)
        {
            try
            {
                var pet = await _serviceManager.PetService.GetPetByIdAsync(id);
                if (pet == null)
                    return NotFound();

                // Ensure the pet belongs to the current user
                var userId = await GetCurrentUserIdAsync();
                if (pet.UserId != userId)
                    return Forbid();

                return Ok(_mapper.Map<PetDTO>(pet));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PetDTO>> AddPet(CreatePetDTO createPetDTO)
        {
            if (createPetDTO == null)
                return BadRequest();

            try
            {
                var pet = _mapper.Map<Pet>(createPetDTO);
                
                // Set the user ID
                pet.UserId = await GetCurrentUserIdAsync();

                var createdPet = await _serviceManager.PetService.AddPetAsync(pet);
                return CreatedAtAction(nameof(GetPet), new { id = createdPet.Id }, _mapper.Map<PetDTO>(createdPet));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, UpdatePetDTO updatePetDTO)
        {
            if (updatePetDTO == null)
                return BadRequest();

            try
            {
                var existingPet = await _serviceManager.PetService.GetPetByIdAsync(id);
                if (existingPet == null)
                    return NotFound();

                // Ensure the pet belongs to the current user
                var userId = await GetCurrentUserIdAsync();
                if (existingPet.UserId != userId)
                    return Forbid();

                // Map the update DTO to the existing entity
                _mapper.Map(updatePetDTO, existingPet);
                
                var result = await _serviceManager.PetService.UpdatePetAsync(existingPet);
                if (!result)
                    return StatusCode(500, "Failed to update pet");

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            try
            {
                var pet = await _serviceManager.PetService.GetPetByIdAsync(id);
                if (pet == null)
                    return NotFound();

                // Ensure the pet belongs to the current user
                var userId = await GetCurrentUserIdAsync();
                if (pet.UserId != userId)
                    return Forbid();

                var result = await _serviceManager.PetService.DeletePetAsync(id);
                if (!result)
                    return StatusCode(500, "Failed to delete pet");

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
*/ 