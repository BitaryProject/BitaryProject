using Core.Common.Specifications;

//using Core.Domain.Contracts.NewModule;
//using Core.Domain.Entities.ClinicEntities;
//using Microsoft.EntityFrameworkCore;
//using Infrastructure.Persistence.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Infrastructure.Persistence.Repositories.NewModule
//{
//    public class ClinicRepository : NewModuleGenericRepository<Clinic, int>, IClinicRepository
//    {
//        public ClinicRepository(NewModuleContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<Clinic>> GetClinicsByCityAsync(string city)
//        {
//            return await GetAllAsQueryable()
//                         .Where(c => c.Address.City.ToLowerInvariant().Contains(city.ToLowerInvariant()))
//                         .ToListAsync();
//        }

//        public async Task<IEnumerable<Clinic>> GetClinicsByNameAsync(string clinicName)
//        {
//            return await GetAllAsQueryable()
//                         .Where(c => c.ClinicName.ToLowerInvariant().Contains(clinicName.ToLowerInvariant()))
//                         .ToListAsync();
//        }
//        public async Task<IEnumerable<Clinic>> GetTopRatedClinicsAsync(int count)
//        {
//            return await _context.Clinics
//                .OrderByDescending(c => c.Rating)
//                .Take(count)
//                .ToListAsync();
//        }


//        public async Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetPagedClinicsAsync(Specifications<Clinic> specifications, int pageIndex, int pageSize)
//        {
//            return await GetPagedAsync(specifications, pageIndex, pageSize);
//        }

//        public async Task<IEnumerable<Clinic>> SearchClinicsInRadiusAsync(string city, int radiusKm)
//        {
//            throw new NotImplementedException("");
//        }
//    }
//}








