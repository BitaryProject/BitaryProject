/*
using Infrastructure.Presentation.Extensions;
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetPets()
        {
            try
            {
                var pets = await _petService.GetAllPetsAsync();
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all pets");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> GetPagedPets(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedPets = await _petService.GetPagedPetsAsync(pageIndex, pageSize);
                return Ok(pagedPets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged pets");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetPet(Guid id)
        {
            try
            {
                var pet = await _petService.GetPetByIdAsync(id);
                
                if (pet == null)
                    return NotFound($"Pet with ID {id} not found");
                
                // Check if the user is authorized to view this pet
                var userId = User.GetUserId();
                var isAuthorized = await _petService.IsUserAuthorizedForPetAsync(userId, id);
                
                if (!isAuthorized && !User.IsInRole("Admin"))
                    return Forbid("You are not authorized to view this pet");
                
                return Ok(pet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pet {PetId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> GetPetsByOwner(
            Guid ownerId, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                
                // Only allow users to view their own pets or admin access
                if (userId != ownerId.ToString() && !User.IsInRole("Admin"))
                    return Forbid("You are not authorized to view these pets");
                
                var pets = await _petService.GetPetsByOwnerAsync(ownerId, pageIndex, pageSize);
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pets for owner {OwnerId}", ownerId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> GetCurrentUserPets(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var ownerId = Guid.Parse(userId);
                
                var pets = await _petService.GetPetsByOwnerAsync(ownerId, pageIndex, pageSize);
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pets for current user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult<PetDTO>> CreatePet(PetCreateDTO petDto)
        {
            try
            {
                var userId = User.GetUserId();
                var ownerId = Guid.Parse(userId);
                
                // Only allow users to create pets for themselves unless they're an admin
                if (!User.IsInRole("Admin") && petDto.OwnerId != ownerId)
                    return Forbid("You can only create pets for yourself");
                
                var pet = await _petService.CreatePetAsync(petDto);
                
                return CreatedAtAction(
                    nameof(GetPet), 
                    new { id = pet.Id }, 
                    pet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pet");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult> UpdatePet(Guid id, PetUpdateDTO petDto)
        {
            try
            {
                var userId = User.GetUserId();
                var pet = await _petService.GetPetByIdAsync(id);
                
                if (pet == null)
                    return NotFound($"Pet with ID {id} not found");
                
                // Only allow users to update their own pets unless they're an admin
                if (!User.IsInRole("Admin") && 
                    (!Guid.TryParse(userId, out var ownerGuid) || pet.OwnerId != ownerGuid))
                {
                    return Forbid("You can only update your own pets");
                }
                
                await _petService.UpdatePetAsync(id, petDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pet {PetId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "PetOwner,Admin")]
        public async Task<ActionResult> DeletePet(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var pet = await _petService.GetPetByIdAsync(id);
                
                if (pet == null)
                    return NotFound($"Pet with ID {id} not found");
                
                // Only allow users to delete their own pets unless they're an admin
                if (!User.IsInRole("Admin") && 
                    (!Guid.TryParse(userId, out var ownerGuid) || pet.OwnerId != ownerGuid))
                {
                    return Forbid("You can only delete your own pets");
                }
                
                await _petService.DeletePetAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pet {PetId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}/medical-records")]
        public async Task<ActionResult<PagedResultDTO<MedicalRecordDTO>>> GetPetMedicalRecords(
            Guid id, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pet = await _petService.GetPetByIdAsync(id);
                
                if (pet == null)
                    return NotFound($"Pet with ID {id} not found");
                
                var userId = User.GetUserId();
                var isAuthorized = await _petService.IsUserAuthorizedForPetAsync(userId, id);
                
                if (!isAuthorized && !User.IsInRole("Admin") && !User.IsInRole("Doctor"))
                    return Forbid("You are not authorized to view medical records for this pet");
                
                var records = await _petService.GetPetMedicalRecordsAsync(id, pageIndex, pageSize);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records for pet {PetId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}/appointments")]
        public async Task<ActionResult<PagedResultDTO<AppointmentDTO>>> GetPetAppointments(
            Guid id, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pet = await _petService.GetPetByIdAsync(id);
                
                if (pet == null)
                    return NotFound($"Pet with ID {id} not found");
                
                var userId = User.GetUserId();
                var isAuthorized = await _petService.IsUserAuthorizedForPetAsync(userId, id);
                
                if (!isAuthorized && !User.IsInRole("Admin") && !User.IsInRole("Doctor"))
                    return Forbid("You are not authorized to view appointments for this pet");
                
                var appointments = await _petService.GetPetAppointmentsAsync(id, pageIndex, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for pet {PetId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}/prescriptions")]
        public async Task<ActionResult<PagedResultDTO<PrescriptionDTO>>> GetPetPrescriptions(
            Guid id, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pet = await _petService.GetPetByIdAsync(id);
                
                if (pet == null)
                    return NotFound($"Pet with ID {id} not found");
                
                var userId = User.GetUserId();
                var isAuthorized = await _petService.IsUserAuthorizedForPetAsync(userId, id);
                
                if (!isAuthorized && !User.IsInRole("Admin") && !User.IsInRole("Doctor") && !User.IsInRole("Pharmacist"))
                    return Forbid("You are not authorized to view prescriptions for this pet");
                
                var prescriptions = await _petService.GetPetPrescriptionsAsync(id, pageIndex, pageSize);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for pet {PetId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<PagedResultDTO<PetDTO>>> SearchPets(
            [FromQuery] string searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term cannot be empty");
                
                var results = await _petService.SearchPetsAsync(searchTerm, pageIndex, pageSize);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching pets with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("species")]
        public async Task<ActionResult<IEnumerable<string>>> GetPetSpecies()
        {
            try
            {
                var species = await _petService.GetAllPetSpeciesAsync();
                return Ok(species);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pet species");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("breeds/{species}")]
        public async Task<ActionResult<IEnumerable<string>>> GetPetBreeds(string species)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(species))
                    return BadRequest("Species cannot be empty");
                
                var breeds = await _petService.GetPetBreedsBySpeciesAsync(species);
                return Ok(breeds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving breeds for species {Species}", species);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
} 


*/
