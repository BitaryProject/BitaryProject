using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Abstractions
{
    public interface IPetOwnerService
    {
        Task<PetOwnerDTO> GetByIdAsync(Guid id);
        Task<PetOwnerDTO> GetByUserIdAsync(string userId);
        Task<PetOwnerDTO> GetByEmailAsync(string email);
        Task<HealthcarePagedResultDTO<PetOwnerDTO>> GetAllPetOwnersAsync(int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PetOwnerDTO>> GetPetOwnersByNameAsync(string name, int pageIndex, int pageSize);
        Task<HealthcarePagedResultDTO<PetOwnerDTO>> GetPetOwnersByEmailAsync(string email, int pageIndex, int pageSize);
        Task<PetOwnerDTO> CreatePetOwnerAsync(PetOwnerCreateUpdateDTO petOwnerCreateDto);
        Task<PetOwnerDTO> UpdatePetOwnerAsync(Guid id, PetOwnerCreateUpdateDTO petOwnerUpdateDto);
        Task DeletePetOwnerAsync(Guid id);
        Task<PetOwnerDTO> GetPetOwnerByUserIdAsync(string userId);
        Task<PetOwnerDTO> GetPetOwnerByIdAsync(Guid id);
        Task<PetOwnerDTO> UpdatePetOwnerProfileAsync(string userId, PetOwnerUpdateDTO model);
    }
} 
