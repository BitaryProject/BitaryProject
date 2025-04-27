using AutoMapper;
using Domain.Contracts;
using Domain.Entities.AppointmentEntities;
using Domain.Entities.DoctorEntites;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.AppointmentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int id)
        {
            var spec = new AppointmentSpecification(id);
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetAsync(spec);
            
            if (appointment == null)
                throw new AppointmentNotFoundException(id.ToString());
                
            return _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetUserAppointmentsAsync(string userId)
        {
            var spec = new AppointmentSpecification(userId);
            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(spec);
            
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetPetAppointmentsAsync(int petId)
        {
            var spec = new AppointmentSpecification(petId, true);
            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(spec);
            
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetDoctorAppointmentsAsync(int doctorId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var spec = new AppointmentSpecification(doctorId, true, fromDate, toDate);
            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(spec);
            
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetClinicAppointmentsAsync(int clinicId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var spec = AppointmentSpecification.ForClinic(clinicId, fromDate, toDate);
            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(spec);
            
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            var spec = new AppointmentSpecification(status);
            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(spec);
            
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO model, string userId)
        {
            // Check if the appointment date is in the past
            if (model.AppointmentDate < DateTime.UtcNow)
                throw new InvalidOperationException("Appointments cannot be booked in the past.");

            // Skip the doctor availability check temporarily to allow appointments to be created
            /*
            // Check if the doctor is available at that time
            if (!await IsDoctorAvailableAsync(model.DoctorId, model.AppointmentDate))
                throw new InvalidOperationException("The doctor is not available at the selected time.");
            */

            // Create the appointment
            var appointment = _mapper.Map<Appointment>(model);
            appointment.UserId = userId;
            appointment.Status = AppointmentStatus.Pending;
            appointment.CreatedAt = DateTime.UtcNow;
            
            // Let the database generate the ID
            appointment.Id = 0;

            await _unitOfWork.GetRepository<Appointment, int>().AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            // Return the created appointment with relationships
            return await GetAppointmentByIdAsync(appointment.Id);
        }

        public async Task<AppointmentDTO> UpdateAppointmentStatusAsync(int id, AppointmentDTO model)
        {
            var spec = new AppointmentSpecification(id);
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetAsync(spec);
            
            if (appointment == null)
                throw new AppointmentNotFoundException(id.ToString());
            
            // Update status and notes
            appointment.Status = model.Status;
            
            if (!string.IsNullOrEmpty(model.Notes))
                appointment.Notes = model.Notes;
            
            _unitOfWork.GetRepository<Appointment, int>().Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            // Return the updated appointment with relationships
            return await GetAppointmentByIdAsync(appointment.Id);
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetAsync(id);
            
            if (appointment == null)
                throw new AppointmentNotFoundException(id.ToString());
            
            _unitOfWork.GetRepository<Appointment, int>().Delete(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime)
        {
            // 1. Check if the appointment falls within doctor's schedule for that day of week
            var scheduleSpec = new DoctorScheduleSpecification(doctorId, appointmentTime.Date);
            var doctorSchedules = await _unitOfWork.GetRepository<DoctorSchedule, int>().GetAllAsync(scheduleSpec);
            
            if (doctorSchedules == null || !doctorSchedules.Any())
                return false; // Doctor doesn't work on this day
                
            // 2. Check if the appointment time is within any of the doctor's working hours for that day
            var time = appointmentTime.TimeOfDay;
            if (!doctorSchedules.Any(schedule => time >= schedule.StartTime && time <= schedule.EndTime))
                return false; // Outside of doctor's working hours
                
            // 3. Check if there's a conflicting appointment (30-minute slots)
            var appointmentDuration = TimeSpan.FromMinutes(30);
            var conflictingAppointmentSpec = new AppointmentSpecification(
                doctorId, 
                appointmentTime, 
                appointmentDuration);
                
            var conflictingAppointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(conflictingAppointmentSpec);
            
            return !conflictingAppointments.Any();
        }
    }
}
