using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.HealthcareEntities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Services.Specifications;
using Shared.Common;
using Shared.HealthcareModels;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IHealthcareUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IHealthcareUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AppointmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AppointmentDTO> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting appointment with ID {Id}", id);
            
            var spec = new AppointmentSpecification(id);
            var appointment = await _unitOfWork.AppointmentRepository.FirstOrDefaultAsync(spec);
            
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");
                
            return _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments for doctor {DoctorId}", doctorId);
            
            var spec = new AppointmentSpecification(doctorId, pageIndex, pageSize);
            var appointments = await _unitOfWork.AppointmentRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.AppointmentRepository.CountAsync(spec);
            
            return new PagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(appointments),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByPetAsync(Guid petId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments for pet {PetId}", petId);
            
            var spec = new AppointmentSpecification(petId, true, pageIndex, pageSize);
            var appointments = await _unitOfWork.AppointmentRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.AppointmentRepository.CountAsync(spec);
            
            return new PagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(appointments),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByClinicAsync(Guid clinicId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments for clinic {ClinicId}", clinicId);
            
            var spec = new AppointmentSpecification(clinicId, "ByClinic", pageIndex, pageSize);
            var appointments = await _unitOfWork.AppointmentRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.AppointmentRepository.CountAsync(spec);
            
            return new PagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(appointments),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByStatusAsync(string status, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments with status {Status}", status);
            
            var spec = new AppointmentSpecification(status, pageIndex, pageSize);
            var appointments = await _unitOfWork.AppointmentRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.AppointmentRepository.CountAsync(spec);
            
            return new PagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(appointments),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<AppointmentDTO>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments between {StartDate} and {EndDate}", startDate, endDate);
            
            var spec = new AppointmentSpecification(startDate, endDate, pageIndex, pageSize);
            var appointments = await _unitOfWork.AppointmentRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.AppointmentRepository.CountAsync(spec);
            
            return new PagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(appointments),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<AppointmentDTO> CreateAppointmentAsync(AppointmentCreateDTO appointmentCreateDto)
        {
            _logger.LogInformation("Creating new appointment for pet {PetId} with doctor {DoctorId}", appointmentCreateDto.PetId, appointmentCreateDto.DoctorId);
            
            // Validate the doctor exists
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(appointmentCreateDto.DoctorId);
            if (doctor == null)
                throw new NotFoundException($"Doctor with ID {appointmentCreateDto.DoctorId} not found");
                
            // Validate the pet exists
            var pet = await _unitOfWork.PetRepository.GetAsync(appointmentCreateDto.PetId);
            if (pet == null)
                throw new NotFoundException($"Pet with ID {appointmentCreateDto.PetId} not found");
                
            // Validate the clinic exists
            var clinic = await _unitOfWork.ClinicRepository.GetAsync(appointmentCreateDto.ClinicId);
            if (clinic == null)
                throw new NotFoundException($"Clinic with ID {appointmentCreateDto.ClinicId} not found");
                
            // Check if the time slot is available
            if (!await IsTimeSlotAvailableAsync(appointmentCreateDto.DoctorId, appointmentCreateDto.AppointmentTime, appointmentCreateDto.DurationMinutes))
                throw new ValidationException(new List<string> { "The selected time slot is not available." });
                
            var appointment = new Appointment
            {
                AppointmentDateTime = appointmentCreateDto.AppointmentTime,
                AppointmentTime = appointmentCreateDto.AppointmentTime, // For backward compatibility
                Duration = TimeSpan.FromMinutes(appointmentCreateDto.DurationMinutes),
                DurationMinutes = appointmentCreateDto.DurationMinutes,
                Status = AppointmentStatus.Scheduled,
                Reason = appointmentCreateDto.Reason,
                Notes = appointmentCreateDto.Notes,
                DoctorId = appointmentCreateDto.DoctorId,
                PetProfileId = appointmentCreateDto.PetId,
                PetId = appointmentCreateDto.PetId, // For backward compatibility
                ClinicId = appointmentCreateDto.ClinicId
            };
            
            await _unitOfWork.AppointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Appointment {Id} created for pet {PetId} with doctor {DoctorId} at clinic {ClinicId}", 
                appointment.Id, appointment.PetProfileId, appointment.DoctorId, appointment.ClinicId);
                
            // Reload the entity with related data
            var spec = new AppointmentSpecification(appointment.Id);
            var createdAppointment = await _unitOfWork.AppointmentRepository.FirstOrDefaultAsync(spec);
            
            return _mapper.Map<AppointmentDTO>(createdAppointment);
        }

        public async Task<AppointmentDTO> UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO appointmentUpdateDto)
        {
            _logger.LogInformation("Updating appointment {Id}", id);
            
            var appointment = await _unitOfWork.AppointmentRepository.GetAsync(id);
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");
                
            // Check if the time slot is available (only if changing appointment time)
            if (appointment.AppointmentDateTime != appointmentUpdateDto.AppointmentTime)
            {
                if (!await IsTimeSlotAvailableAsync(appointment.DoctorId, appointmentUpdateDto.AppointmentTime, appointmentUpdateDto.DurationMinutes))
                    throw new ValidationException(new List<string> { "The selected time slot is not available." });
            }
            
            // Parse the status string to the enum
            AppointmentStatus status;
            if (!Enum.TryParse<AppointmentStatus>(appointmentUpdateDto.Status, out status))
            {
                throw new ValidationException(new List<string> { $"Invalid appointment status: {appointmentUpdateDto.Status}" });
            }
                
            appointment.AppointmentDateTime = appointmentUpdateDto.AppointmentTime;
            appointment.AppointmentTime = appointmentUpdateDto.AppointmentTime; // For backward compatibility
            appointment.Duration = TimeSpan.FromMinutes(appointmentUpdateDto.DurationMinutes);
            appointment.DurationMinutes = appointmentUpdateDto.DurationMinutes;
            appointment.Status = status;
            appointment.Reason = appointmentUpdateDto.Reason;
            appointment.Notes = appointmentUpdateDto.Notes;
            
            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Appointment {Id} updated", id);
            
            // Reload the entity with related data
            var spec = new AppointmentSpecification(id);
            var updatedAppointment = await _unitOfWork.AppointmentRepository.FirstOrDefaultAsync(spec);
            
            return _mapper.Map<AppointmentDTO>(updatedAppointment);
        }

        public async Task<AppointmentDTO> UpdateAppointmentStatusAsync(Guid id, string status)
        {
            _logger.LogInformation("Updating status of appointment {Id} to {Status}", id, status);
            
            var appointment = await _unitOfWork.AppointmentRepository.GetAsync(id);
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");
            
            // Parse the status string to the enum
            AppointmentStatus appointmentStatus;
            if (!Enum.TryParse<AppointmentStatus>(status, out appointmentStatus))
            {
                throw new ValidationException(new List<string> { $"Invalid appointment status: {status}" });
            }
                
            appointment.Status = appointmentStatus;
            
            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Appointment {Id} status updated to {Status}", id, status);
            
            // Reload the entity with related data
            var spec = new AppointmentSpecification(id);
            var updatedAppointment = await _unitOfWork.AppointmentRepository.FirstOrDefaultAsync(spec);
            
            return _mapper.Map<AppointmentDTO>(updatedAppointment);
        }

        public async Task DeleteAppointmentAsync(Guid id)
        {
            _logger.LogInformation("Deleting appointment {Id}", id);
            
            var appointment = await _unitOfWork.AppointmentRepository.GetAsync(id);
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");
                
            _unitOfWork.AppointmentRepository.Delete(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Appointment {Id} deleted", id);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime appointmentTime, int durationMinutes)
        {
            // Get all appointments for the doctor on the same day
            var startOfDay = appointmentTime.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            
            var spec = new AppointmentSpecification(a => 
                a.DoctorId == doctorId && 
                a.AppointmentDateTime >= startOfDay && 
                a.AppointmentDateTime <= endOfDay &&
                a.Status != AppointmentStatus.Cancelled);
                
            var appointmentsOnDay = await _unitOfWork.AppointmentRepository.ListAsync(spec);
            var appointmentEndTime = appointmentTime.AddMinutes(durationMinutes);
            
            // Check if there's any overlap with existing appointments
            foreach (var existingAppointment in appointmentsOnDay)
            {
                var existingAppointmentEndTime = existingAppointment.AppointmentDateTime.AddMinutes(existingAppointment.DurationMinutes);
                
                // Check for overlap
                if ((appointmentTime >= existingAppointment.AppointmentDateTime && appointmentTime < existingAppointmentEndTime) ||
                    (appointmentEndTime > existingAppointment.AppointmentDateTime && appointmentEndTime <= existingAppointmentEndTime) ||
                    (appointmentTime <= existingAppointment.AppointmentDateTime && appointmentEndTime >= existingAppointmentEndTime))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
