using Domain.Contracts;
using Domain.Entities.PetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly StoreContext _context;

        public PetRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            Console.WriteLine($"Getting pet with ID: {id}");

            try
            {
                var pet = await _context.Pets
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    Console.WriteLine($"Pet with ID {id} not found");
                    return null;
                }

                Console.WriteLine($"Found pet: {pet.PetName}");
                return pet;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving pet with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId)
        {
            Console.WriteLine($"Getting pets for user with ID: {userId}");

            try
            {
                var pets = await _context.Pets
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                Console.WriteLine($"Retrieved {pets.Count} pets for user");
                return pets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving pets for user {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Pet> AddPetAsync(Pet pet)
        {
            Console.WriteLine($"Adding new pet: {pet.PetName}");

            try
            {
                // Validate pet data
                if (string.IsNullOrEmpty(pet.PetName))
                {
                    throw new ArgumentException("Pet name is required");
                }

                if (string.IsNullOrEmpty(pet.UserId))
                {
                    throw new ArgumentException("User ID is required");
                }

                // Add the pet to the context
                await _context.Pets.AddAsync(pet);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully added pet with ID: {pet.Id}");
                return pet;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding pet: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdatePetAsync(Pet pet)
        {
            Console.WriteLine($"Updating pet with ID: {pet.Id}");

            try
            {
                // Validate pet data
                if (pet.Id <= 0)
                {
                    throw new ArgumentException("Invalid pet ID");
                }

                if (string.IsNullOrEmpty(pet.PetName))
                {
                    throw new ArgumentException("Pet name is required");
                }

                if (string.IsNullOrEmpty(pet.UserId))
                {
                    throw new ArgumentException("User ID is required");
                }

                // Check if pet exists
                var existingPet = await _context.Pets.FindAsync(pet.Id);
                if (existingPet == null)
                {
                    Console.WriteLine($"Pet with ID {pet.Id} not found for update");
                    return false;
                }

                // Update the entity
                _context.Entry(existingPet).State = EntityState.Detached;
                _context.Pets.Update(pet);
                var result = await _context.SaveChangesAsync() > 0;

                Console.WriteLine($"Pet update result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating pet with ID {pet.Id}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletePetAsync(int id)
        {
            Console.WriteLine($"Deleting pet with ID: {id}");

            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet == null)
                {
                    Console.WriteLine($"Pet with ID {id} not found for deletion");
                    return false;
                }

                _context.Pets.Remove(pet);
                var result = await _context.SaveChangesAsync() > 0;

                Console.WriteLine($"Pet deletion result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting pet with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
} 