using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPetService
    {
        Task<PetDTO> GetByIdAsync(Guid id);
        Task<PagedResultDTO<PetDTO>> GetPetsByOwnerAsync(Guid ownerId, int pageIndex, int pageSize);
        Task<PagedResultDTO<PetDTO>> GetPetsByNameAsync(string name, int pageIndex, int pageSize);
        Task<PagedResultDTO<PetDTO>> GetPetsBySpeciesAndBreedAsync(string species, string breed, int pageIndex, int pageSize);
        Task<PetDTO> CreatePetAsync(PetCreateDTO petCreateDto);
        Task<PetDTO> UpdatePetAsync(Guid id, PetUpdateDTO petUpdateDto);
        Task DeletePetAsync(Guid id);
    }
}
