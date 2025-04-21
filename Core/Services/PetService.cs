using AutoMapper;
using Domain.Contracts;
using Domain.Entities.PetEntities;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Services.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PetService : Services.Abstractions.IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PetService> _logger;

        public PetService(IPetRepository petRepository, IMapper mapper, ILogger<PetService> logger)
        {
            _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Pet> AddPetAsync(Pet pet)
        {
            if (pet == null)
                throw new ArgumentNullException(nameof(pet));

            try
            {
                // Validate pet properties
                if (string.IsNullOrEmpty(pet.PetName))
                    throw new ArgumentException("Pet name is required");
                    
                if (string.IsNullOrEmpty(pet.UserId))
                    throw new ArgumentException("User ID is required");
                
                _logger.LogInformation($"Adding new pet '{pet.PetName}' for user '{pet.UserId}'");
                
                // Use the repository to add the pet
                var createdPet = await _petRepository.AddPetAsync(pet);
                
                _logger.LogInformation($"Successfully added pet '{pet.PetName}' with ID {pet.Id}");
                return createdPet;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Validation error when adding pet: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error when adding pet: {ex.Message}");
                throw new InvalidOperationException($"An unexpected error occurred while adding the pet", ex);
            }
        }

        public async Task<bool> DeletePetAsync(int petId)
        {
            if (petId <= 0)
                throw new ArgumentException("Invalid pet ID", nameof(petId));

            try
            {
                _logger.LogInformation($"Attempting to delete pet with ID {petId}");
                
                var result = await _petRepository.DeletePetAsync(petId);
                
                if (result)
                    _logger.LogInformation($"Successfully deleted pet with ID {petId}");
                else
                    _logger.LogWarning($"Pet with ID {petId} not found for deletion");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error when deleting pet with ID {petId}: {ex.Message}");
                throw new InvalidOperationException($"An unexpected error occurred while deleting the pet", ex);
            }
        }

        public async Task<Pet> GetPetByIdAsync(int petId)
        {
            if (petId <= 0)
                throw new ArgumentException("Invalid pet ID", nameof(petId));

            try
            {
                _logger.LogInformation($"Retrieving pet with ID {petId}");
                
                var pet = await _petRepository.GetPetByIdAsync(petId);

                if (pet == null)
                    _logger.LogWarning($"Pet with ID {petId} not found");
                else
                    _logger.LogInformation($"Successfully retrieved pet with ID {petId}");
                
                return pet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving pet with ID {petId}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve pet: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            try
            {
                _logger.LogInformation($"Retrieving pets for user with ID {userId}");
                
                var pets = await _petRepository.GetPetsByUserIdAsync(userId);
                
                _logger.LogInformation($"Retrieved {pets.Count()} pets for user with ID {userId}");
                return pets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving pets for user with ID {userId}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve pets: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdatePetAsync(Pet pet)
        {
            if (pet == null)
                throw new ArgumentNullException(nameof(pet));
            
            if (pet.Id <= 0)
                throw new ArgumentException("Invalid pet ID", nameof(pet));

            try
            {
                // Validate pet properties
                if (string.IsNullOrEmpty(pet.PetName))
                    throw new ArgumentException("Pet name is required");
                    
                if (string.IsNullOrEmpty(pet.UserId))
                    throw new ArgumentException("User ID is required");
                
                _logger.LogInformation($"Updating pet with ID {pet.Id}");
                
                var result = await _petRepository.UpdatePetAsync(pet);
                
                if (result)
                    _logger.LogInformation($"Successfully updated pet with ID {pet.Id}");
                else
                    _logger.LogWarning($"Pet with ID {pet.Id} not found for update");
                
                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Validation error when updating pet with ID {pet.Id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error when updating pet with ID {pet.Id}: {ex.Message}");
                throw new InvalidOperationException($"An unexpected error occurred while updating the pet", ex);
            }
        }
    }
}
