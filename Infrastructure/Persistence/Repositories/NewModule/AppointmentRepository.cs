using Core.Common.Specifications;

//using Core.Domain.Contracts.NewModule;
//using Core.Domain.Entities.AppointmentEntities;
//using Microsoft.EntityFrameworkCore;
//using Infrastructure.Persistence.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Infrastructure.Persistence.Repositories.NewModule
//{
//    public class AppointmentRepository : NewModuleGenericRepository<Appointment, int>, IAppointmentRepository
//    {
//        public AppointmentRepository(NewModuleContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId)
//        {
//            return await GetAllAsQueryable()
//                         .Where(a => a.UserId == userId)
//                         .ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByPetIdAsync(int petId)
//        {
//            return await GetAllAsQueryable()
//                         .Where(a => a.PetId == petId)
//                         .ToListAsync();
//        }

//        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetPagedAppointmentsAsync(Specifications<Appointment> specifications, int pageIndex, int pageSize)
//        {
//            return await GetPagedAsync(specifications, pageIndex, pageSize);
//        }
//    }
//}








