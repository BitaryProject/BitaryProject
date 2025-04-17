using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IPetService
    {
        Task<PetDTO> GetByIdAsync(Guid id);
        Task<HealthcarePagedResultDTO<PetDTO>> GetPetsByOwnerAsync(Guid ownerId, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PetDTO>> GetPetsByNameAsync(string name, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PetDTO>> GetPetsBySpeciesAndBreedAsync(string species, string breed, int pageIndex, int pageSize);
        Task<PetDTO> CreatePetAsync(PetCreateDTO petCreateDto);
        Task<PetDTO> UpdatePetAsync(Guid id, PetUpdateDTO petUpdateDto);
        Task DeletePetAsync(Guid id);
    }
}

