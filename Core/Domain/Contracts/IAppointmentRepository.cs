using Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IAppointmentRepository : IGenericRepository<Appointment, Guid>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByPetProfileIdAsync(Guid petProfileId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end);
        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetPagedAppointmentsAsync(Specifications<Appointment> specifications, int pageIndex, int pageSize);
        Task<bool> CheckForConflictingAppointmentsAsync(Guid doctorId, DateTime dateTime, TimeSpan duration);
    }
} 