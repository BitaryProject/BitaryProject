using Domain.Entities.PetEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPetRepository
    {
        Task<Pet?> GetPetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId);
        Task<Pet> AddPetAsync(Pet pet);
        Task<bool> UpdatePetAsync(Pet pet);
        Task<bool> DeletePetAsync(int id);
    }
} 