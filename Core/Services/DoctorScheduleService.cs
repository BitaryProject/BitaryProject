using AutoMapper;
using Domain.Contracts;
using Domain.Entities.DoctorEntites;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorScheduleService(
            IUnitOFWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime date)
        {
            var utcDate = date.ToUniversalTime();
            
            var specs = new DoctorScheduleSpecification(doctorId, utcDate.Date);
            var schedules = await _unitOfWork.GetRepository<DoctorSchedule, int>().GetAllAsync(specs);

            if (!schedules.Any())
                return false;

            var time = utcDate.TimeOfDay;
            
            // Check if the requested time falls within any of the doctor's schedules for that day
            return schedules.Any(schedule => time >= schedule.StartTime && time <= schedule.EndTime);
        }

        public async Task<DoctorScheduleDTO> AddScheduleAsync(int doctorId, DoctorScheduleDTO dto)
        {
            // Verify doctor exists
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());
                
            // Check for overlapping schedules on the same day
            var specs = new DoctorScheduleSpecification(doctorId, dto.ScheduleDate.Date);
            var existingSchedules = await _unitOfWork.GetRepository<DoctorSchedule, int>().GetAllAsync(specs);
            
            // Check for time conflicts with existing schedules
            foreach (var existing in existingSchedules)
            {
                bool overlaps = (dto.StartTime <= existing.EndTime && dto.EndTime >= existing.StartTime);
                
                if (overlaps)
                {
                    throw new InvalidOperationException(
                        $"Schedule conflicts with existing schedule from {existing.StartTime:hh\\:mm} to {existing.EndTime:hh\\:mm}");
                }
            }

            var schedule = _mapper.Map<DoctorSchedule>(dto);
            
            // Ensure ID is set to 0 to let the database generate it
            schedule.Id = 0;
            schedule.DoctorId = doctorId;

            if (schedule.StartTime >= schedule.EndTime)
                throw new InvalidOperationException("Start time must be before end time");

            await _unitOfWork.GetRepository<DoctorSchedule, int>().AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();
            
            // Create result DTO with doctor name
            var resultDto = _mapper.Map<DoctorScheduleDTO>(schedule);
            resultDto = resultDto with { DoctorName = doctor.Name };
            
            return resultDto;
        }
        
        public async Task<IEnumerable<DoctorScheduleDTO>> GetDoctorSchedulesAsync(int doctorId)
        {
            // Verify doctor exists
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());
            
            var specs = DoctorScheduleSpecification.GetByDoctorId(doctorId);
            var schedules = await _unitOfWork.GetRepository<DoctorSchedule, int>().GetAllAsync(specs);
            
            var scheduleDtos = _mapper.Map<List<DoctorScheduleDTO>>(schedules);
            
            // Set doctor name for all schedules
            for (int i = 0; i < scheduleDtos.Count; i++)
            {
                scheduleDtos[i] = scheduleDtos[i] with { DoctorName = doctor.Name };
            }
            
            // Order by date and time
            return scheduleDtos.OrderBy(s => s.ScheduleDate.Date).ThenBy(s => s.StartTime).ToList();
        }
        
        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _unitOfWork.GetRepository<DoctorSchedule, int>().GetAsync(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Schedule not found");
                
            _unitOfWork.GetRepository<DoctorSchedule, int>().Delete(schedule);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }
    }
}