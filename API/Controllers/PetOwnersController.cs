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
    public class PetOwnersController : ControllerBase
    {
        private readonly IPetOwnerService _petOwnerService;
        private readonly ILogger<PetOwnersController> _logger;

        public PetOwnersController(
            IPetOwnerService petOwnerService,
            ILogger<PetOwnersController> logger)
        {
            _petOwnerService = petOwnerService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetOwnerDTO>> GetById(Guid id)
        {
            try
            {
                var petOwner = await _petOwnerService.GetByIdAsync(id);
                return Ok(petOwner);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet owner {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PetOwnerDTO>> GetByUserId(string userId)
        {
            try
            {
                var petOwner = await _petOwnerService.GetByUserIdAsync(userId);
                return Ok(petOwner);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet owner with user ID {UserId}", userId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<PetOwnerDTO>> GetByEmail(string email)
        {
            try
            {
                var petOwner = await _petOwnerService.GetByEmailAsync(email);
                return Ok(petOwner);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet owner with email {Email}", email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<PetOwnerDTO>>> GetAll(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var petOwners = await _petOwnerService.GetPetOwnersAsync(pageIndex, pageSize);
                return Ok(petOwners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all pet owners");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PetOwnerDTO>> Create(PetOwnerCreateUpdateDTO createDto)
        {
            try
            {
                var petOwner = await _petOwnerService.CreatePetOwnerAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = petOwner.Id }, petOwner);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pet owner");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PetOwnerDTO>> Update(Guid id, PetOwnerCreateUpdateDTO updateDto)
        {
            try
            {
                var petOwner = await _petOwnerService.UpdatePetOwnerAsync(id, updateDto);
                return Ok(petOwner);
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
                _logger.LogError(ex, "Error updating pet owner {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _petOwnerService.DeletePetOwnerAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pet owner {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 