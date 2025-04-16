using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
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
        public AppointmentRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPetProfileIdAsync(Guid petProfileId)
        {
            return await _context.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.PetProfileId == petProfileId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByClinicIdAsync(Guid clinicId)
        {
            return await _context.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.ClinicId == clinicId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Appointments
                .Include(a => a.PetProfile)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.AppointmentDateTime >= start && a.AppointmentDateTime <= end)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetPagedAppointmentsAsync(Specifications<Appointment> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }

        public async Task<bool> CheckForConflictingAppointmentsAsync(Guid doctorId, DateTime dateTime, TimeSpan duration)
        {
            var end = dateTime.Add(duration);
            
            // Check if there are any appointments with the same doctor that overlap with the given time range
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status != AppointmentStatus.Cancelled)
                .AnyAsync(a => 
                    (a.AppointmentDateTime <= dateTime && dateTime < a.AppointmentDateTime.Add(a.Duration)) ||
                    (a.AppointmentDateTime < end && end <= a.AppointmentDateTime.Add(a.Duration)) ||
                    (dateTime <= a.AppointmentDateTime && a.AppointmentDateTime < end)
                );
        }
    }
} 