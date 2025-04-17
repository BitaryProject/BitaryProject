using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories.HealthcareRepositories
{
    public class AppointmentRepository : GenericRepository<Appointment, Guid>, IAppointmentRepository
    {
        private readonly StoreContext _storeContext;

        public AppointmentRepository(StoreContext context) : base(context)
        {
            _storeContext = context;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPetProfileIdAsync(Guid petProfileId)
        {
            return await _storeContext.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Clinic)
                .Where(a => a.PetProfileId == petProfileId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _storeContext.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Clinic)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByClinicIdAsync(Guid clinicId)
        {
            return await _storeContext.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Clinic)
                .Where(a => a.ClinicId == clinicId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _storeContext.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Clinic)
                .Where(a => a.AppointmentDateTime >= start && a.AppointmentDateTime <= end)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetPagedAppointmentsAsync(
            ISpecification<Appointment> specification, int pageIndex, int pageSize)
        {
            var query = _storeContext.Appointments.AsQueryable();

            // Apply specification
            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            // Apply includes
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply paging
            var skip = (pageIndex - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // Execute query
            var appointments = await query.ToListAsync();

            return (appointments, totalCount);
        }

        public async Task<bool> CheckForConflictingAppointmentsAsync(Guid doctorId, DateTime dateTime, TimeSpan duration)
        {
            var end = dateTime.Add(duration);
            
            return await _storeContext.Appointments
                .Where(a => a.DoctorId == doctorId && 
                           a.Status != AppointmentStatus.Cancelled &&
                           a.Status != AppointmentStatus.NoShow)
                .AnyAsync(a => 
                    (a.AppointmentDateTime <= dateTime && dateTime < a.AppointmentDateTime.Add(a.Duration)) ||
                    (a.AppointmentDateTime < end && end <= a.AppointmentDateTime.Add(a.Duration)) ||
                    (dateTime <= a.AppointmentDateTime && a.AppointmentDateTime < end)
                );
        }
    }
}