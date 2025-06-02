using AutoMapper;
using Domain.Entities.ClinicEntities;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Entities.SecurityEntities;
using Domain.Entities.DoctorEntites;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;
using Services.Specifications;
using Shared.ClinicModels;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Services
{
    public class ClinicService : IClinicService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ClinicService(
            IUnitOFWork unitOfWork,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ClinicDTO?> GetClinicByIdAsync(int clinicId)
        {
            var spec = new ClinicSpecification(clinicId);
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(spec);
            
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());
                
            var clinicDto = _mapper.Map<ClinicDTO>(clinic);
            
            // Set the owner name
            var owner = await _userManager.FindByIdAsync(clinic.OwnerId);
            if (owner != null)
            {
                clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}".Trim();
            }
            
            // Get all active doctors for this clinic
            var doctorSpec = new DoctorSpecification(d => d.ClinicId == clinicId);
            var doctors = await _unitOfWork.GetRepository<Doctor, int>().GetAllAsync(doctorSpec);
            
            // Map doctors to DTOs and include their schedules
            var doctorDtos = new List<DoctorDTO>();
            foreach (var doctor in doctors)
            {
                var doctorDto = _mapper.Map<DoctorDTO>(doctor) with { ClinicName = clinic.ClinicName };
                doctorDtos.Add(doctorDto);
            }
            
            clinicDto.Doctors = doctorDtos;
            
            return clinicDto;
        }

        public async Task<IEnumerable<ClinicDTO>> GetAllClinicsAsync()
        {
            var clinics = await _unitOfWork.GetRepository<Clinic, int>().GetAllAsync();
            var clinicDtos = _mapper.Map<IEnumerable<ClinicDTO>>(clinics);
            
            foreach (var clinicDto in clinicDtos)
            {
                var owner = await _userManager.FindByIdAsync(clinicDto.OwnerId);
                if (owner != null)
                {
                    clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}";
                }
            }
            
            return clinicDtos;
        }

        public async Task<ClinicDTO> CreateClinicAsync(ClinicRequestDTO model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var clinic = _mapper.Map<Clinic>(model);
            clinic.OwnerId = userId;
            clinic.Status = ClinicStatus.Pending;
            
            await _unitOfWork.GetRepository<Clinic, int>().AddAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
            
            var clinicDto = _mapper.Map<ClinicDTO>(clinic);
            clinicDto.OwnerName = $"{user.FirstName} {user.LastName}";
            
            return clinicDto;
        }

        public async Task<ClinicDTO?> UpdateClinicAsync(int clinicId, ClinicDTO model)
        {
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            _mapper.Map(model, clinic);
            _unitOfWork.GetRepository<Clinic, int>().Update(clinic);
            await _unitOfWork.SaveChangesAsync();
            
            var owner = await _userManager.FindByIdAsync(clinic.OwnerId);
            var clinicDto = _mapper.Map<ClinicDTO>(clinic);
            
            if (owner != null)
            {
                clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}";
            }
            
            return clinicDto;
        }

        public async Task<bool> DeleteClinicAsync(int clinicId)
        {
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            _unitOfWork.GetRepository<Clinic, int>().Delete(clinic);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ClinicDTO>> GetPendingClinicsAsync()
        {
            var clinics = await _unitOfWork.GetRepository<Clinic, int>().GetAllAsync();
            var pendingClinics = clinics.Where(c => c.Status == ClinicStatus.Pending).ToList();
            
            var clinicDtos = _mapper.Map<IEnumerable<ClinicDTO>>(pendingClinics);
            
            foreach (var clinicDto in clinicDtos)
            {
                var owner = await _userManager.FindByIdAsync(clinicDto.OwnerId);
                if (owner != null)
                {
                    clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}";
                }
            }
            
            return clinicDtos;
        }

        public async Task<ClinicDTO> UpdateClinicStatusAsync(int clinicId, ClinicStatusUpdateDTO statusUpdate)
        {
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            clinic.Status = statusUpdate.Status;
            _unitOfWork.GetRepository<Clinic, int>().Update(clinic);
            await _unitOfWork.SaveChangesAsync();
            
            var owner = await _userManager.FindByIdAsync(clinic.OwnerId);
            var clinicDto = _mapper.Map<ClinicDTO>(clinic);
            
            if (owner != null)
            {
                clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}";
            }
            
            return clinicDto;
        }

        public async Task<IEnumerable<ClinicDTO>> GetClinicsByOwnerIdAsync(string ownerId)
        {
            var clinics = await _unitOfWork.GetRepository<Clinic, int>().GetAllAsync();
            var ownerClinics = clinics.Where(c => c.OwnerId == ownerId).ToList();
            
            var clinicDtos = _mapper.Map<IEnumerable<ClinicDTO>>(ownerClinics);
            
            var owner = await _userManager.FindByIdAsync(ownerId);
            if (owner != null)
            {
                string ownerName = $"{owner.FirstName} {owner.LastName}";
                foreach (var clinicDto in clinicDtos)
                {
                    clinicDto.OwnerName = ownerName;
                }
            }
            
            return clinicDtos;
        }

        public async Task<ClinicDTO> AddDoctorToClinicAsync(int clinicId, int doctorId)
        {
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            var doctor = await _unitOfWork.GetRepository<Doctor, int>().GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());

            doctor.ClinicId = clinicId;
            doctor.Clinic = clinic;
            
            _unitOfWork.GetRepository<Doctor, int>().Update(doctor);
            await _unitOfWork.SaveChangesAsync();
            
            var owner = await _userManager.FindByIdAsync(clinic.OwnerId);
            var clinicDto = _mapper.Map<ClinicDTO>(clinic);
            
            if (owner != null)
            {
                clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}";
            }
            
            return clinicDto;
        }

        public async Task<ClinicDTO> ApproveClinicAsync(int clinicId)
        {
            var statusUpdate = new ClinicStatusUpdateDTO
            {
                Status = ClinicStatus.Approved
            };

            return await UpdateClinicStatusAsync(clinicId, statusUpdate);
        }

        public async Task<ClinicDTO> RejectClinicAsync(int clinicId)
        {
            var statusUpdate = new ClinicStatusUpdateDTO
            {
                Status = ClinicStatus.Rejected
            };

            return await UpdateClinicStatusAsync(clinicId, statusUpdate);
        }

        public async Task<ClinicDTO?> UpdateClinicBasicInfoAsync(int clinicId, ClinicUpdateDTO model)
        {
            var clinic = await _unitOfWork.GetRepository<Clinic, int>().GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            // Use mapper to only update allowed properties (ClinicName and Address)
            _mapper.Map(model, clinic);
            
            _unitOfWork.GetRepository<Clinic, int>().Update(clinic);
            await _unitOfWork.SaveChangesAsync();
            
            var owner = await _userManager.FindByIdAsync(clinic.OwnerId);
            var clinicDto = _mapper.Map<ClinicDTO>(clinic);
            
            if (owner != null)
            {
                clinicDto.OwnerName = $"{owner.FirstName} {owner.LastName}";
            }
            
            return clinicDto;
        }
    }
}
