using AutoMapper;
using Domain.Entities.AppointmentEntities;
using Domain.Contracts.NewModule; 
using Domain.Exceptions;     
using Services.Abstractions;
using Shared.AppointmentModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IMapper mapper;
      //  private IAppointmentService appointmentRepository1;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
        }

      /*  public AppointmentService(IAppointmentService appointmentRepository1, IMapper mapper)
        {
            this.appointmentRepository1 = appointmentRepository1;
            this.mapper = mapper;
        }
      */

        public async Task<AppointmentDTO?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            var appointment = await appointmentRepository.GetAsync(appointmentId);
            if (appointment == null)
                throw new AppointmentNotFoundException(appointmentId.ToString());
            return mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByUserIdAsync(string userId)
        {
            var appointments = await appointmentRepository.GetAppointmentsByUserIdAsync(userId);
            return mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPetIdAsync(Guid petId)
        {
            var appointments = await appointmentRepository.GetAppointmentsByPetIdAsync(petId);
            return mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

      /*  public async Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO model)
        {
            var appointment = mapper.Map<Appointment>(model);
            appointment.Id = Guid.NewGuid();
            await appointmentRepository.AddAsync(appointment);
            return mapper.Map<AppointmentDTO>(appointment);
        }
        */

        public async Task<AppointmentDTO?> UpdateAppointmentAsync(Guid appointmentId, AppointmentDTO model)
        {
            var appointment = await appointmentRepository.GetAsync(appointmentId);
            if (appointment == null)
                throw new AppointmentNotFoundException(appointmentId.ToString());

            mapper.Map(model, appointment);
            appointmentRepository.Update(appointment);

            return mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<bool> DeleteAppointmentAsync(Guid appointmentId)
        {
            var appointment = await appointmentRepository.GetAsync(appointmentId);
            if (appointment == null)
                throw new AppointmentNotFoundException(appointmentId.ToString());

            appointmentRepository.Delete(appointment);
            return true;
        }

        public async Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO model)
        {
        
            if (model.AppointmentDate < DateTime.UtcNow)
                throw new Exception("Appointments cannot be booked at a previous date.");


            TimeSpan appointmentDuration = TimeSpan.FromMinutes(30);
            DateTime appointmentStart = model.AppointmentDate;
            DateTime appointmentEnd = model.AppointmentDate.Add(appointmentDuration);

            /*
                        var existingAppointments = await appointmentRepository.GetAllAsQueryable()
                           .Where(a => a.DoctorId == model.DoctorId)
                           .ToListAsync();
                        foreach (var existing in existingAppointments)
                        {
                            DateTime existingStart = existing.AppointmentDate;
                            DateTime existingEnd = existingStart.Add(appointmentDuration);

                            // لو وقت البداية أو النهاية بيتداخلوا، نرفض الحجز
                            if (appointmentStart < existingEnd && existingStart < appointmentEnd)
                                throw new Exception(" another appointment scheduled at the same time with this doctor. Choose another time.");
                        }
            */

            var conflictingAppointments = await appointmentRepository.GetAllAsQueryable()
                .Where(a => a.DoctorId == model.DoctorId &&
                  a.AppointmentDate < appointmentEnd &&
                  a.AppointmentDate.AddMinutes(30) > appointmentStart)
                .AnyAsync();

            if (conflictingAppointments)
                throw new Exception("There is another appointment booked at the same time with the selected doctor.");
           

            var appointment = mapper.Map<Appointment>(model);
            appointment.Id = Guid.NewGuid();
            await appointmentRepository.AddAsync(appointment);

            return mapper.Map<AppointmentDTO>(appointment);
        }

    }
}
