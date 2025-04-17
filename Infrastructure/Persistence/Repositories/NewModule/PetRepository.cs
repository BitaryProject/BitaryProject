using Core.Common.Specifications;

//using Core.Domain.Contracts.NewModule;
//using Core.Domain.Entities.PetEntities;
//using Microsoft.EntityFrameworkCore;
//using Infrastructure.Persistence.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Infrastructure.Persistence.Repositories.NewModule
//{
//    public class PetRepository : NewModuleGenericRepository<Pet, int>, IPetRepository
//    {
//        public PetRepository(NewModuleContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<Pet>> GetPetsByUserIdAsync(string userId)
//        {
//            return await GetAllAsQueryable()
//                         .Where(p => p.UserId == userId)
//                         .ToListAsync();
//        }

//        public async Task<(IEnumerable<Pet> Pets, int TotalCount)> GetPagedPetsAsync(Specifications<Pet> specifications, int pageIndex, int pageSize)
//        {
//            return await GetPagedAsync(specifications, pageIndex, pageSize);
//        }
//    }
//}








