using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ClinicEntities;
using Domain.Entities.DoctorEntites;
using Domain.Entities.SecurityEntities;
using Domain.Entities.AppointmentEntities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
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
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public DoctorService(
            IUnitOFWork unitOfWork,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<DoctorDTO?> GetDoctorByIdAsync(int doctorId)
        {
            var spec = new DoctorSpecification(doctorId);
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(spec);
            
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());
                
            var doctorDto = _mapper.Map<DoctorDTO>(doctor);
            
            if (doctor.ClinicId > 0 && doctor.Clinic != null)
            {
                doctorDto = doctorDto with { ClinicName = doctor.Clinic.ClinicName };
            }
            else if (doctor.ClinicId > 0)
            {
                var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(doctor.ClinicId);
                if (clinic != null)
                {
                    doctorDto = doctorDto with { ClinicName = clinic.ClinicName };
                }
            }
            
            return doctorDto;
        }

        public async Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            Expression<Func<Doctor, bool>> criteria = d => d.Specialty.Contains(specialty);
            var specs = new DoctorSpecification(criteria);
            var doctors = await _unitOfWork.GetRepository<Doctor, int>().GetAllAsync(specs);
            
            var doctorDtos = _mapper.Map<IEnumerable<DoctorDTO>>(doctors).ToList();
            
            // Get clinic names for each doctor
            for (int i = 0; i < doctorDtos.Count; i++)
            {
                var doctorDto = doctorDtos[i];
                if (doctorDto.ClinicId > 0)
                {
                    var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(doctorDto.ClinicId);
                    if (clinic != null)
                    {
                        doctorDtos[i] = doctorDto with { ClinicName = clinic.ClinicName };
                    }
                }
            }
            
            return doctorDtos;
        }

        public async Task<DoctorDTO> CreateDoctorAsync(DoctorDTO model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);
                
            // Check if user's role is Doctor
            if (user.UserRole != Role.Doctor)
                throw new InvalidOperationException("Only users with Doctor role can create a doctor profile");
                
            // Check if doctor profile already exists for this user
            var existingDoctorSpec = new DoctorSpecification(userId);
            var existingDoctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(existingDoctorSpec);
            
            if (existingDoctor != null)
                throw new InvalidOperationException("Doctor profile already exists for this user");

            var doctor = _mapper.Map<Doctor>(model);
            
            // Let the database generate the ID - don't set it manually
            doctor.Id = 0;
            doctor.UserId = userId;
            
            await _unitOfWork.GetRepository<Doctor, int>().AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
            
            // Log the created doctor's ID
            Console.WriteLine($"Created doctor with ID: {doctor.Id}");

            return await GetDoctorByIdAsync(doctor.Id);
        }

        public async Task<DoctorDTO?> UpdateDoctorAsync(int doctorId, DoctorDTO model)
        {
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());

            _mapper.Map(model, doctor);
            
            // Ensure we don't change the ID during updates
            doctor.Id = doctorId;
            
            _unitOfWork.GetRepository<Doctor, int>().Update(doctor);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetDoctorByIdAsync(doctor.Id);
        }

        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());

            // Check for appointments
            var appointmentSpec = new AppointmentSpecification(a => a.DoctorId == doctorId);
            var appointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(appointmentSpec);

            if (appointments.Any())
            {
                // Delete all appointments for this doctor
                foreach (var appointment in appointments)
                {
                    _unitOfWork.GetRepository<Appointment, int>().Delete(appointment);
                }
            }

            // Now safe to delete the doctor
            _unitOfWork.GetRepository<Doctor, int>().Delete(doctor);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        
        public async Task<DoctorDTO?> GetDoctorByUserIdAsync(string userId)
        {
            var specs = new DoctorSpecification(userId);
            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(specs);
            
            if (doctor == null)
                return null;
                
            return await GetDoctorByIdAsync(doctor.Id);
        }
    }
}
