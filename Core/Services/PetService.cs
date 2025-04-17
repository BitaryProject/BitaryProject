//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services
//{
//    using AutoMapper;
//    using Domain.Contracts.NewModule;
//    using Core.Domain.Entities.PetEntities;
//    using Domain.Exceptions;
//    using Shared.PetModels;
//    using System;
//    using System.Collections.Generic;
//    using System.Threading.Tasks;

//    namespace Services
//    {
      
//        public class PetService : IPetService
//        {
//            private readonly IPetRepository petRepository;
//            private readonly IMapper mapper;
//          //  private IPetService petRepository1;

//            public PetService(IPetRepository petRepository, IMapper mapper)
//            {
//                this.petRepository = petRepository;
//                this.mapper = mapper;
//            }

//           /*
//            public PetService(IPetService petRepository1, IMapper mapper)
//            {
//                this.petRepository1 = petRepository1;
//                this.mapper = mapper;
//            }
//           */
//            public async Task<PetProfileDTO> CreatePetAsync(PetProfileDTO petDto)
//            {
//                var pet = mapper.Map<Pet>(petDto);
//                await petRepository.AddAsync(pet);

//                return mapper.Map<PetProfileDTO>(pet);
//            }

//            public async Task<PetProfileDTO?> GetPetAsync(string id)
//            {
//                if (!int.TryParse(id, out var petGuid))
//                    throw new Exception($"Invalid pet Id format: {id}");

//                var pet = await petRepository.GetAsync(petGuid);
//                if (pet == null)
//                    throw new PetNotFoundException(id);

//                return mapper.Map<PetProfileDTO>(pet);
//            }

//            public async Task<IEnumerable<PetProfileDTO>> GetPetsByUserIdAsync(string userId)
//            {
//                var pets = await petRepository.GetPetsByUserIdAsync(userId);
//                return mapper.Map<IEnumerable<PetProfileDTO>>(pets);
//            }

//            public async Task<PetProfileDTO?> UpdatePetAsync(string petId, PetProfileDTO petDto)
//            {
//                if (!int.TryParse(petId, out var petGuid))
//                    throw new Exception($"Invalid pet Id format: {petId}");

//                var existingPet = await petRepository.GetAsync(petGuid);
//                if (existingPet == null)
//                    throw new PetNotFoundException(petId);

//                existingPet.PetName = petDto.PetName;
//                existingPet.BirthDate = petDto.BirthDate;

//                existingPet.Gender = petDto.Gender;

//                existingPet.Color = petDto.Color;
//                existingPet.Avatar = petDto.Avatar;

//                petRepository.Update(existingPet);

//                return mapper.Map<PetProfileDTO>(existingPet);
//            }

//            public async Task<bool> DeletePetAsync(string petId)
//            {
//                if (!int.TryParse(petId, out var petGuid))
//                    throw new Exception($"Invalid pet Id format: {petId}");

//                var pet = await petRepository.GetAsync(petGuid);
//                if (pet == null)
//                    throw new PetNotFoundException(petId);

//                petRepository.Delete(pet);

//                return true;
//            }
//        }
//    }

//}
