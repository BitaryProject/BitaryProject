using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Contracts;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Core.Services.Abstractions;
using Core.Services.Specifications;
using Shared.HealthcareModels;
using Core.Services.Specifications.Base;
using System.Linq.Expressions;
using Core.Common.Specifications;

namespace Core.Services
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

        public async Task<AppointmentDTO> GetAppointmentByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting appointment with ID {Id}", id);
            
            var spec = new AppointmentSpecification(id);
            var appointment = await _unitOfWork.AppointmentRepository.GetAsync(spec);
            
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");
                
            return _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPetIdAsync(Guid petId)
        {
            _logger.LogInformation("Getting appointments for pet {PetId}", petId);
            
            var spec = new AppointmentSpecification(petId, true, 1, int.MaxValue);
            var appointments = await _unitOfWork.AppointmentRepository.GetAllWithSpecAsync(spec);
            
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDoctorIdAsync(Guid doctorId, DateTime? date = null)
        {
            _logger.LogInformation("Getting appointments for doctor {DoctorId}", doctorId);
            
            AppointmentSpecification spec;
            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1).AddTicks(-1);
                spec = new AppointmentSpecification(a => a.DoctorId == doctorId && 
                                                    a.AppointmentDateTime >= startDate && 
                                                    a.AppointmentDateTime <= endDate);
            }
            else
            {
                spec = new AppointmentSpecification(doctorId, 1, int.MaxValue);
            }
            
            var appointments = await _unitOfWork.AppointmentRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByClinicIdAsync(Guid clinicId, DateTime? date = null)
        {
            _logger.LogInformation("Getting appointments for clinic {ClinicId}", clinicId);
            
            AppointmentSpecification spec;
            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1).AddTicks(-1);
                spec = new AppointmentSpecification(a => a.ClinicId == clinicId && 
                                                   a.AppointmentDateTime >= startDate && 
                                                   a.AppointmentDateTime <= endDate);
            }
            else
            {
                spec = new AppointmentSpecification(clinicId, "ByClinic", 1, int.MaxValue);
            }
            
            var appointments = await _unitOfWork.AppointmentRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByOwnerIdAsync(Guid ownerId, DateTime? date = null)
        {
            _logger.LogInformation("Getting appointments for owner {OwnerId}", ownerId);
            
            var petIds = await _unitOfWork.PetProfileRepository.GetPetIdsByOwnerIdAsync(ownerId);
            if (!petIds.Any())
                return new List<AppointmentDTO>();
            
            var appointments = new List<AppointmentDTO>();
            foreach (var petId in petIds)
            {
                var petAppointments = await GetAppointmentsByPetIdAsync(petId);
                appointments.AddRange(petAppointments);
            }
            
            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1).AddTicks(-1);
                appointments = appointments.Where(a => a.AppointmentDateTime >= startDate && 
                                                 a.AppointmentDateTime <= endDate).ToList();
            }
            
            return appointments;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetUpcomingAppointmentsByPetIdAsync(Guid petId)
        {
            _logger.LogInformation("Getting upcoming appointments for pet {PetId}", petId);
            
            var spec = new AppointmentSpecification(a => a.PetProfileId == petId && 
                                                  a.AppointmentDateTime > DateTime.UtcNow && 
                                                  (a.Status == AppointmentStatus.Scheduled.ToString() || 
                                                   a.Status == AppointmentStatus.Confirmed.ToString()));
            
            var appointments = await _unitOfWork.AppointmentRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments for doctor {DoctorId}", doctorId);
            
            var spec = new AppointmentSpecification(doctorId, pageIndex, pageSize);
            var result = await _unitOfWork.AppointmentRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByPetAsync(Guid petId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments for pet {PetId}", petId);
            
            var spec = new AppointmentSpecification(petId, true, pageIndex, pageSize);
            var result = await _unitOfWork.AppointmentRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByClinicAsync(Guid clinicId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments for clinic {ClinicId}", clinicId);
            
            var spec = new AppointmentSpecification(clinicId, "ByClinic", pageIndex, pageSize);
            var result = await _unitOfWork.AppointmentRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByStatusAsync(string status, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments with status {Status}", status);
            
            var spec = new AppointmentSpecification(status, pageIndex, pageSize);
            var result = await _unitOfWork.AppointmentRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting appointments between {StartDate} and {EndDate}", startDate, endDate);
            
            var spec = new AppointmentSpecification(startDate, endDate, pageIndex, pageSize);
            var result = await _unitOfWork.AppointmentRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<AppointmentDTO> CreateAppointmentAsync(AppointmentCreateDTO appointmentCreateDto)
        {
            try
            {
                ValidateCreateAppointmentDto(appointmentCreateDto);

                _logger.LogInformation("Creating new appointment for pet {PetId} with doctor {DoctorId}", 
                    appointmentCreateDto.PetProfileId, appointmentCreateDto.DoctorId);
                
                // Validate doctor exists and is available
                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(appointmentCreateDto.DoctorId);
                if (doctor == null)
                    throw new NotFoundException($"Doctor with ID {appointmentCreateDto.DoctorId} not found");
                if (!doctor.IsAvailable)
                    throw new ValidationException(new List<string> { "The selected doctor is not currently available for appointments" });
                    
                // Validate pet exists
                var pet = await _unitOfWork.PetProfileRepository.GetByIdAsync(appointmentCreateDto.PetProfileId);
                if (pet == null)
                    throw new NotFoundException($"Pet with ID {appointmentCreateDto.PetProfileId} not found");
                    
                // Validate clinic exists and is active
                var clinic = await _unitOfWork.ClinicRepository.GetByIdAsync(appointmentCreateDto.ClinicId);
                if (clinic == null)
                    throw new NotFoundException($"Clinic with ID {appointmentCreateDto.ClinicId} not found");
                    
                // Validate doctor works at clinic
                if (!await _unitOfWork.DoctorRepository.IsDoctorAssociatedWithClinicAsync(appointmentCreateDto.DoctorId, appointmentCreateDto.ClinicId))
                    throw new ValidationException(new List<string> { "The selected doctor is not associated with the selected clinic" });
                    
                // Check if the time slot is available
                if (!await IsTimeSlotAvailableAsync(appointmentCreateDto.DoctorId, appointmentCreateDto.AppointmentDateTime, appointmentCreateDto.Duration))
                    throw new ValidationException(new List<string> { "The selected time slot is not available" });
                    
                // Validate appointment time is in the future
                if (appointmentCreateDto.AppointmentDateTime <= DateTime.UtcNow)
                    throw new ValidationException(new List<string> { "Appointment time must be in the future" });

                var appointment = new Appointment
                {
                    AppointmentDateTime = appointmentCreateDto.AppointmentDateTime,
                    Duration = appointmentCreateDto.Duration ?? TimeSpan.FromHours(1),
                    Status = AppointmentStatus.Scheduled,
                    Reason = appointmentCreateDto.Reason,
                    Notes = appointmentCreateDto.Notes,
                    DoctorId = appointmentCreateDto.DoctorId,
                    PetProfileId = appointmentCreateDto.PetProfileId,
                    ClinicId = appointmentCreateDto.ClinicId,
                    FollowUpToAppointmentId = appointmentCreateDto.FollowUpToAppointmentId
                };

                await _unitOfWork.AppointmentRepository.AddAsync(appointment);
                
                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database error while creating appointment");
                    throw new DatabaseOperationException("Failed to create the appointment", ex);
                }
                
                _logger.LogInformation("Appointment {Id} created for pet {PetId} with doctor {DoctorId} at clinic {ClinicId}", 
                    appointment.Id, appointment.PetProfileId, appointment.DoctorId, appointment.ClinicId);
                    
                // Reload the entity with related data
                var spec = new AppointmentSpecification(appointment.Id);
                var createdAppointment = await _unitOfWork.AppointmentRepository.GetAsync(ConvertSpecification(spec));
                
                return _mapper.Map<AppointmentDTO>(createdAppointment);
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Unexpected error while creating appointment");
                throw new AppointmentOperationException("An unexpected error occurred while creating the appointment", ex);
            }
        }

        public async Task<AppointmentDTO> UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO appointmentUpdateDto)
        {
            try
            {
                ValidateUpdateAppointmentDto(appointmentUpdateDto);

                _logger.LogInformation("Updating appointment {Id}", id);
                
                var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                    throw new NotFoundException($"Appointment with ID {id} not found");
                
                // Validate appointment can be updated
                if (appointment.Status == AppointmentStatus.Completed || 
                    appointment.Status == AppointmentStatus.Cancelled || 
                    appointment.Status == AppointmentStatus.NoShow)
                {
                    throw new ValidationException(new List<string> 
                    { 
                        "Cannot update an appointment that is completed, cancelled, or marked as no-show" 
                    });
                }
                
                // Check if the time slot is available (only if changing appointment time)
                if (appointment.AppointmentDateTime != appointmentUpdateDto.AppointmentDateTime)
                {
                    // Validate new time is in the future
                    if (appointmentUpdateDto.AppointmentDateTime <= DateTime.UtcNow)
                        throw new ValidationException(new List<string> { "Appointment time must be in the future" });

                    if (!await IsTimeSlotAvailableAsync(appointment.DoctorId, appointmentUpdateDto.AppointmentDateTime, appointmentUpdateDto.Duration))
                        throw new ValidationException(new List<string> { "The selected time slot is not available" });
                }
                
                // Update the appointment
                appointment.AppointmentDateTime = appointmentUpdateDto.AppointmentDateTime;
                appointment.Duration = appointmentUpdateDto.Duration;
                
                if (!string.IsNullOrEmpty(appointmentUpdateDto.Status))
                {
                    if (!Enum.TryParse<AppointmentStatus>(appointmentUpdateDto.Status, out var status))
                        throw new ValidationException(new List<string> { $"Invalid appointment status: {appointmentUpdateDto.Status}" });
                    appointment.Status = status;
                }
                
                if (!string.IsNullOrEmpty(appointmentUpdateDto.Reason))
                    appointment.Reason = appointmentUpdateDto.Reason;
                
                if (!string.IsNullOrEmpty(appointmentUpdateDto.Notes))
                    appointment.Notes = appointmentUpdateDto.Notes;
                
                _unitOfWork.AppointmentRepository.Update(appointment);
                
                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database error while updating appointment {Id}", id);
                    throw new DatabaseOperationException($"Failed to update appointment with ID {id}", ex);
                }
                
                _logger.LogInformation("Appointment {Id} updated", id);
                
                // Reload the entity with related data
                var spec = new AppointmentSpecification(id);
                var updatedAppointment = await _unitOfWork.AppointmentRepository.GetAsync(ConvertSpecification(spec));
                
                return _mapper.Map<AppointmentDTO>(updatedAppointment);
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Unexpected error while updating appointment {Id}", id);
                throw new AppointmentOperationException($"An unexpected error occurred while updating appointment with ID {id}", ex);
            }
        }

        public async Task<AppointmentDTO> UpdateAppointmentStatusAsync(Guid id, string status)
        {
            _logger.LogInformation("Updating status of appointment {Id} to {Status}", id, status);
            
            var appointment = await _unitOfWork.AppointmentRepository.GetAsync(id);
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");
            
            // Check if status transition is valid
            if (status.Equals(Core.Domain.Entities.HealthcareEntities.AppointmentStatus.Cancelled.ToString(), StringComparison.OrdinalIgnoreCase) || 
                status.Equals(Core.Domain.Entities.HealthcareEntities.AppointmentStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase) || 
                status.Equals(Core.Domain.Entities.HealthcareEntities.AppointmentStatus.NoShow.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (appointment.Status == Core.Domain.Entities.HealthcareEntities.AppointmentStatus.Cancelled)
                {
                    throw new AppointmentOperationException($"Cannot change status of a cancelled appointment");
                }
            }
            
            // Convert string status to enum
            if (Enum.TryParse<Core.Domain.Entities.HealthcareEntities.AppointmentStatus>(status, true, out var statusEnum))
            {
                appointment.Status = statusEnum;
            }
            else
            {
                throw new ArgumentException($"Invalid appointment status: {status}");
            }
            
            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Appointment {Id} status updated to {Status}", id, status);
            
            // Reload the entity with related data
            var spec = new AppointmentSpecification(id);
            var updatedAppointment = await _unitOfWork.AppointmentRepository.GetAsync(ConvertSpecification(spec));
            
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

        public async Task<bool> CancelAppointmentAsync(Guid id, string cancellationReason)
        {
            _logger.LogInformation("Canceling appointment {Id}", id);
            
            var appointment = await _unitOfWork.AppointmentRepository.GetAsync(id);
            if (appointment == null)
                throw new NotFoundException($"Appointment with ID {id} not found");

            // Only allow cancellation of scheduled or confirmed appointments
            if (appointment.Status != AppointmentStatus.Scheduled && 
                appointment.Status != AppointmentStatus.Confirmed)
            {
                throw new ValidationException(new List<string> 
                { 
                    "Cannot cancel an appointment that is already completed, cancelled, or marked as no-show" 
                });
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.Notes = (appointment.Notes ?? "") + 
                $"\nCancelled on {DateTime.UtcNow:g} UTC\nReason: {cancellationReason}";

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Appointment {Id} cancelled", id);
            return true;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime appointmentTime, TimeSpan duration)
        {
            // Get all appointments for the doctor on the same day
            var startOfDay = appointmentTime.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            
            var spec = new AppointmentSpecification(a => 
                a.DoctorId == doctorId && 
                a.AppointmentDateTime >= startOfDay && 
                a.AppointmentDateTime <= endOfDay &&
                a.Status != AppointmentStatus.Cancelled);
                
            var appointmentsOnDay = await _unitOfWork.AppointmentRepository.GetAllAsync(ConvertSpecification(spec));
            var appointmentEndTime = appointmentTime.Add(duration);
            
            // Check if there's any overlap with existing appointments
            foreach (var existingAppointment in appointmentsOnDay)
            {
                var existingEndTime = existingAppointment.AppointmentDateTime.Add(existingAppointment.Duration);
                
                // Check for overlap
                if ((appointmentTime >= existingAppointment.AppointmentDateTime && appointmentTime < existingEndTime) ||
                    (appointmentEndTime > existingAppointment.AppointmentDateTime && appointmentEndTime <= existingEndTime) ||
                    (appointmentTime <= existingAppointment.AppointmentDateTime && appointmentEndTime >= existingEndTime))
                {
                    return false;
                }
            }
            
            return true;
        }

        public async Task<bool> IsPetOwnedByUserAsync(Guid petId, string userId)
        {
            if (petId == Guid.Empty || string.IsNullOrEmpty(userId))
                return false;
            
            var petProfile = await _unitOfWork.PetRepository.GetByIdAsync(petId);
            if (petProfile == null)
                return false;
            
            var petOwner = await _unitOfWork.PetOwnerRepository.GetByUserIdAsync(userId);
            if (petOwner == null)
                return false;
            
            return petProfile.OwnerId == petOwner.Id;
        }

        public async Task<HealthcarePagedResultDTO<AppointmentDTO>> GetAllAppointmentsAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting all appointments (page {PageIndex}, size {PageSize})", pageIndex, pageSize);
            
            var spec = new AppointmentSpecification();
            var result = await _unitOfWork.AppointmentRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<AppointmentDTO>
            {
                Items = _mapper.Map<List<AppointmentDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        private void ValidateCreateAppointmentDto(AppointmentCreateDTO dto)
        {
            var errors = new List<string>();

            if (dto.AppointmentDateTime == default)
                errors.Add("Appointment date and time is required");

            if (dto.Duration <= TimeSpan.Zero)
                errors.Add("Appointment duration must be positive");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                errors.Add("Appointment reason is required");

            if (dto.PetProfileId == Guid.Empty)
                errors.Add("Pet profile ID is required");

            if (dto.ClinicId == Guid.Empty)
                errors.Add("Clinic ID is required");

            if (dto.DoctorId == Guid.Empty)
                errors.Add("Doctor ID is required");

            if (errors.Any())
                throw new ValidationException(errors);
        }

        private void ValidateUpdateAppointmentDto(AppointmentUpdateDTO dto)
        {
            var errors = new List<string>();

            if (dto.AppointmentDateTime == default)
                errors.Add("Appointment date and time is required");

            if (dto.Duration <= TimeSpan.Zero)
                errors.Add("Appointment duration must be positive");

            if (!string.IsNullOrEmpty(dto.Status) && !Enum.TryParse<AppointmentStatus>(dto.Status, out _))
                errors.Add($"Invalid appointment status: {dto.Status}");

            if (errors.Any())
                throw new ValidationException(errors);
        }

        // Helper method to convert from AppointmentSpecification to BaseSpecification<Appointment>
        private Core.Common.Specifications.ISpecification<Appointment> ConvertSpecification(AppointmentSpecification spec)
        {
            // Create a custom specification adapter that implements the service specification interface
            var customSpec = new CustomAppointmentSpecification(spec.Criteria);
            
            // Copy over all the properties/settings
            foreach (var include in spec.Includes)
                customSpec.AddInclude(include);
            
            foreach (var includeString in spec.IncludeStrings)
                customSpec.AddInclude(includeString);
            
            if (spec.OrderBy != null)
                customSpec.ApplyOrderBy(spec.OrderBy);
            
            if (spec.OrderByDescending != null)
                customSpec.ApplyOrderByDescending(spec.OrderByDescending);
            
            if (spec.IsPagingEnabled)
                customSpec.ApplyPaging(spec.Skip, spec.Take);
            
            return customSpec;
        }
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }

    // Custom exceptions
    public class AppointmentOperationException : Exception
    {
        public AppointmentOperationException(string message, Exception innerException = null) 
            : base(message, innerException)
        {
        }
    }

    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException(string message, Exception innerException = null) 
            : base(message, innerException)
        {
        }
    }

    // Custom specification implementation to bridge the gap between specs
    public class CustomAppointmentSpecification : Core.Common.Specifications.ISpecification<Appointment>
    {
        private readonly Expression<Func<Appointment, bool>> _criteria;
        private readonly List<Expression<Func<Appointment, object>>> _includes = new();
        private readonly List<string> _includeStrings = new();
        private Expression<Func<Appointment, object>> _orderBy;
        private Expression<Func<Appointment, object>> _orderByDescending;
        private Expression<Func<Appointment, object>> _groupBy;
        private int _take;
        private int _skip;
        private bool _isPagingEnabled;

        public CustomAppointmentSpecification(Expression<Func<Appointment, bool>> criteria)
        {
            _criteria = criteria;
        }

        public Expression<Func<Appointment, bool>> Criteria => _criteria;
        public List<Expression<Func<Appointment, object>>> Includes => _includes;
        public List<string> IncludeStrings => _includeStrings;
        public Expression<Func<Appointment, object>> OrderBy => _orderBy;
        public Expression<Func<Appointment, object>> OrderByDescending => _orderByDescending;
        public Expression<Func<Appointment, object>> GroupBy => _groupBy;
        public int Take => _take;
        public int Skip => _skip;
        public bool IsPagingEnabled => _isPagingEnabled;

        public void AddCriteria(Expression<Func<Appointment, bool>> criteria)
        {
            // Not implemented in this context
        }

        public void AddInclude(Expression<Func<Appointment, object>> includeExpression)
        {
            _includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            _includeStrings.Add(includeString);
        }

        public void AddOrderBy(Expression<Func<Appointment, object>> orderByExpression)
        {
            _orderBy = orderByExpression;
        }

        public void ApplyOrderBy(Expression<Func<Appointment, object>> orderByExpression)
        {
            _orderBy = orderByExpression;
        }

        public void ApplyOrderByDescending(Expression<Func<Appointment, object>> orderByDescendingExpression)
        {
            _orderByDescending = orderByDescendingExpression;
        }

        public void ApplyPaging(int skip, int take)
        {
            _skip = skip;
            _take = take;
            _isPagingEnabled = true;
        }
    }
}
