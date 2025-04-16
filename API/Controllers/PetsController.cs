using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Shared.HealthcareModels;
using System;
using System.Threading.Tasks;
using Domain.Exceptions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetsController> _logger;

        public PetsController(
            IPetService petService,
            ILogger<PetsController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetById(Guid id)
        {
            try
            {
                var pet = await _petService.GetByIdAsync(id);
                return Ok(pet);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> GetByOwner(
            Guid ownerId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pets = await _petService.GetPetsByOwnerAsync(ownerId, pageIndex, pageSize);
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pets for owner {OwnerId}", ownerId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> GetByName(
            string name, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pets = await _petService.GetPetsByNameAsync(name, pageIndex, pageSize);
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pets with name {Name}", name);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("species-breed")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> GetBySpeciesAndBreed(
            [FromQuery] string species,
            [FromQuery] string breed, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pets = await _petService.GetPetsBySpeciesAndBreedAsync(species, breed, pageIndex, pageSize);
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pets with species {Species} and breed {Breed}", species, breed);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PetDTO>> Create(PetCreateDTO createDto)
        {
            try
            {
                var pet = await _petService.CreatePetAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pet");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PetDTO>> Update(Guid id, PetUpdateDTO updateDto)
        {
            try
            {
                var pet = await _petService.UpdatePetAsync(id, updateDto);
                return Ok(pet);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pet {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _petService.DeletePetAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pet {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 