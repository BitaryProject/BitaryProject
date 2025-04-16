using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPetOwnerService
    {
        Task<PetOwnerDTO> GetByIdAsync(Guid id);
        Task<PetOwnerDTO> GetByUserIdAsync(string userId);
        Task<PetOwnerDTO> GetByEmailAsync(string email);
        Task<PagedResultDTO<PetOwnerDTO>> GetPetOwnersAsync(int pageIndex, int pageSize);
        Task<PetOwnerDTO> CreatePetOwnerAsync(PetOwnerCreateUpdateDTO petOwnerCreateDto);
        Task<PetOwnerDTO> UpdatePetOwnerAsync(Guid id, PetOwnerCreateUpdateDTO petOwnerUpdateDto);
        Task DeletePetOwnerAsync(Guid id);
    }
} 