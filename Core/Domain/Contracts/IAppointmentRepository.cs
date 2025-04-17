using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IAppointmentRepository : IGenericRepository<Appointment, Guid>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPetProfileIdAsync(Guid petProfileId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPetOwnerIdAsync(Guid petOwnerId);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(string status);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsForDoctorAsync(Guid doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByClinicIdAsync(Guid clinicId);
        Task<bool> CheckForConflictingAppointmentsAsync(DateTime startTime, DateTime endTime, Guid doctorId);
        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetPagedAppointmentsAsync(ISpecification<Appointment> specifications, int pageIndex, int pageSize);
        Task<Appointment> GetAppointmentWithDetailsAsync(Guid id);
    }
} 

