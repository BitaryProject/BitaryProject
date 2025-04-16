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
    public class DoctorRepository : GenericRepository<Doctor, Guid>, IDoctorRepository
    {
        public DoctorRepository(StoreContext context) : base(context)
        {
        }

        public async Task<Doctor> GetDoctorByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByClinicIdAsync(Guid clinicId)
        {
            return await _context.Doctors
                .Include(d => d.Clinic)
                .Where(d => d.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization)
        {
            return await _context.Doctors
                .Include(d => d.Clinic)
                .Where(d => d.Specialization.ToLower() == specialization.ToLower())
                .ToListAsync();
        }

        public async Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(Specifications<Doctor> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }

        public async Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime appointmentDateTime)
        {
            // Check if doctor has any existing appointments at the same time
            // Assuming each appointment is 30 minutes long
            var appointmentEndTime = appointmentDateTime.AddMinutes(30);
            
            var conflictingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                       a.Status != AppointmentStatus.Cancelled &&
                       ((a.AppointmentDateTime <= appointmentDateTime && appointmentDateTime < a.AppointmentDateTime.Add(a.Duration)) ||
                        (a.AppointmentDateTime < appointmentEndTime && appointmentEndTime <= a.AppointmentDateTime.Add(a.Duration)) ||
                        (appointmentDateTime <= a.AppointmentDateTime && a.AppointmentDateTime < appointmentEndTime)))
                .AnyAsync();
                
            return !conflictingAppointments;
        }
    }
} 