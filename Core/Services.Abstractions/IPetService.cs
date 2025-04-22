using Domain.Entities.PetEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPetService
    {
        Task<Pet> AddPetAsync(Pet pet);
        Task<bool> DeletePetAsync(int petId);
        Task<Pet> GetPetByIdAsync(int petId);
        Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId);
        Task<bool> UpdatePetAsync(Pet pet);
    }
}
