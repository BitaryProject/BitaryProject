using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetProfilesController : ControllerBase
    {
        private readonly IPetProfileService _petProfileService;
        private readonly IPetOwnerService _petOwnerService;
        
        public PetProfilesController(
            IPetProfileService petProfileService,
            IPetOwnerService petOwnerService)
        {
            _petProfileService = petProfileService;
            _petOwnerService = petOwnerService;
        }
        
        // Get pet profile by ID - accessible to both pet owners and doctors
        [HttpGet("{id}")]
        [Authorize(Roles = "PetOwner,Doctor")]
        public async Task<ActionResult<PetProfileDTO>> GetPetProfile(Guid id)
        {
            try
            {
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(id);
                
                // Pet owners can only view their own pets
                if (User.IsInRole("PetOwner"))
                {
                    var userId = User.GetUserId();
                    var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                    
                    if (petOwner == null)
                        return NotFound("Pet owner profile not found");
                    
                    if (petProfile.OwnerId != petOwner.Id)
                        return Forbid("You do not have permission to view this pet profile");
                }
                
                return Ok(petProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Get all pet profiles for a pet owner - only accessible to the pet owner themselves
        [HttpGet("my-pets")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetMyPetProfiles()
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(petOwner.Id);
                return Ok(petProfiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Create a new pet profile - only accessible to pet owners
        [HttpPost]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<PetProfileDTO>> CreatePetProfile(PetProfileCreateDTO petProfileDto)
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                var petProfile = await _petProfileService.CreatePetProfileAsync(petOwner.Id, petProfileDto);
                return CreatedAtAction(nameof(GetPetProfile), new { id = petProfile.Id }, petProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Update a pet profile - only accessible to the pet owner
        [HttpPut("{id}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<PetProfileDTO>> UpdatePetProfile(Guid id, PetProfileUpdateDTO petProfileDto)
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                // Verify ownership of the pet
                var existingPetProfile = await _petProfileService.GetPetProfileByIdAsync(id);
                if (existingPetProfile == null)
                    return NotFound($"Pet profile with ID {id} not found");
                
                if (existingPetProfile.OwnerId != petOwner.Id)
                    return Forbid("You can only update your own pets' profiles");
                
                var petProfile = await _petProfileService.UpdatePetProfileAsync(id, petProfileDto);
                return Ok(petProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Delete a pet profile - only accessible to the pet owner
        [HttpDelete("{id}")]
        [Authorize(Roles = "PetOwner")]
        public async Task<ActionResult<bool>> DeletePetProfile(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                
                if (petOwner == null)
                    return NotFound("Pet owner profile not found");
                
                // Verify ownership of the pet
                var existingPetProfile = await _petProfileService.GetPetProfileByIdAsync(id);
                if (existingPetProfile == null)
                    return NotFound($"Pet profile with ID {id} not found");
                
                if (existingPetProfile.OwnerId != petOwner.Id)
                    return Forbid("You can only delete your own pets' profiles");
                
                var result = await _petProfileService.DeletePetProfileAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Endpoint for doctors to view pet profiles during appointments
        [HttpGet("doctor/pet/{petId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PetProfileDTO>> GetPetProfileForDoctor(Guid petId)
        {
            try
            {
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(petId);
                if (petProfile == null)
                    return NotFound($"Pet profile with ID {petId} not found");
                
                return Ok(petProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 