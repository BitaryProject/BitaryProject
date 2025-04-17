/*
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PetProfilesController : ControllerBase
    {
        private readonly IPetProfileService _petProfileService;
        private readonly ILogger<PetProfilesController> _logger;

        public PetProfilesController(
            IPetProfileService petProfileService,
            ILogger<PetProfilesController> logger)
        {
            _petProfileService = petProfileService ?? throw new ArgumentNullException(nameof(petProfileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetAllPetProfiles()
        {
            try
            {
                var petProfiles = await _petProfileService.GetAllPetProfilesAsync();
                return Ok(petProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all pet profiles");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResultDTO<PetProfileDTO>>> GetPagedPetProfiles(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedPetProfiles = await _petProfileService.GetPagedPetProfilesAsync(pageIndex, pageSize);
                return Ok(pagedPetProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged pet profiles");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetProfileDTO>> GetPetProfileById(Guid id)
        {
            try
            {
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(id);

                if (petProfile == null)
                    return NotFound($"Pet profile with ID {id} not found");

                return Ok(petProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pet profile {PetProfileId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetPetProfilesByOwnerId(string ownerId)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerId))
                    return BadRequest("Owner ID cannot be empty");

                var petProfiles = await _petProfileService.GetPetProfilesByOwnerIdAsync(ownerId);
                return Ok(petProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pet profiles for owner {OwnerId}", ownerId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PetProfileDTO>> CreatePetProfile(PetProfileCreateDTO petProfileDto)
        {
            try
            {
                var petProfile = await _petProfileService.CreatePetProfileAsync(petProfileDto);

                return CreatedAtAction(
                    nameof(GetPetProfileById),
                    new { id = petProfile.Id },
                    petProfile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pet profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePetProfile(Guid id, PetProfileUpdateDTO petProfileDto)
        {
            try
            {
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(id);

                if (petProfile == null)
                    return NotFound($"Pet profile with ID {id} not found");

                await _petProfileService.UpdatePetProfileAsync(id, petProfileDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pet profile {PetProfileId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePetProfile(Guid id)
        {
            try
            {
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(id);

                if (petProfile == null)
                    return NotFound($"Pet profile with ID {id} not found");

                await _petProfileService.DeletePetProfileAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pet profile {PetProfileId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDTO<PetProfileDTO>>> SearchPetProfiles(
            [FromQuery] string searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term cannot be empty");

                var results = await _petProfileService.SearchPetProfilesAsync(searchTerm, pageIndex, pageSize);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching pet profiles with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("species/{species}")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetPetProfilesBySpecies(string species)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(species))
                    return BadRequest("Species cannot be empty");

                var petProfiles = await _petProfileService.GetPetProfilesBySpeciesAsync(species);
                return Ok(petProfiles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pet profiles for species {Species}", species);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("breed/{breed}")]
        public async Task<ActionResult<IEnumerable<PetProfileDTO>>> GetPetProfilesByBreed(string breed)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(breed))
                    return BadRequest("Breed cannot be empty");

                var petProfiles = await _petProfileService.GetPetProfilesByBreedAsync(breed);
                return Ok(petProfiles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pet profiles for breed {Breed}", breed);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}/update-picture")]
        public async Task<ActionResult> UpdatePetProfilePicture(Guid id, [FromBody] string pictureUrl)
        {
            try
            {
                var petProfile = await _petProfileService.GetPetProfileByIdAsync(id);

                if (petProfile == null)
                    return NotFound($"Pet profile with ID {id} not found");

                await _petProfileService.UpdatePetProfilePictureAsync(id, pictureUrl);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating picture for pet profile {PetProfileId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 


*/
