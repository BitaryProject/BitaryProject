using Core.Common.Specifications;

//using Core.Domain.Contracts.NewModule;
//using Core.Domain.Entities.MedicalRecordEntites;
//using Microsoft.EntityFrameworkCore;
//using Infrastructure.Persistence.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Infrastructure.Persistence.Repositories.NewModule
//{
//    public class MedicalRecordRepository : NewModuleGenericRepository<MedicalRecord, int>, IMedicalRecordRepository
//    {
//        public MedicalRecordRepository(NewModuleContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<MedicalRecord>> GetRecordsByPetIdAsync(int petId)
//        {
//            return await GetAllAsQueryable()
//                         .Where(r => r.PetId == petId)
//                         .ToListAsync();
//        }

//        public async Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> GetPagedMedicalRecordsAsync(Specifications<MedicalRecord> specifications, int pageIndex, int pageSize)
//        {
//            return await GetPagedAsync(specifications, pageIndex, pageSize);
//        }
//    }
//}








