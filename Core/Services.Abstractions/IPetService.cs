/*using Shared.PetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPetService
    {
        Task<PetProfileDTO> CreatePetAsync(PetProfileDTO petDto);
        Task<PetProfileDTO?> GetPetAsync(string id);
        Task<IEnumerable<PetProfileDTO>> GetPetsByUserIdAsync(string userId);
        Task<PetProfileDTO?> UpdatePetAsync(string petId, PetProfileDTO petDto);
        Task<bool> DeletePetAsync(string petId);
    }

}
*/