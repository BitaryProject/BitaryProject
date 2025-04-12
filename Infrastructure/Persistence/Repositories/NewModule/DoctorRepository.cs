//using Domain.Contracts.NewModule;
//using Domain.Entities.DoctorEntites;
//using Microsoft.EntityFrameworkCore;
//using Persistence.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Persistence.Repositories.NewModule
//{
//    public class DoctorRepository : NewModuleGenericRepository<Doctor, int>, IDoctorRepository
//    {
//        public DoctorRepository(NewModuleContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyAsync(string specialty)
//        {
//            return await GetAllAsQueryable()
//                         .Where(d => d.Specialty.ToLowerInvariant().Contains(specialty.ToLowerInvariant()))
//                         .ToListAsync();
//        }

//        public async Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(Specifications<Doctor> specifications, int pageIndex, int pageSize)
//        {
//            return await GetPagedAsync(specifications, pageIndex, pageSize);
//        }
//    }
//}
