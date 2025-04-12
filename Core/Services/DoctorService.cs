/*using AutoMapper;
using Domain.Contracts.NewModule; 
using Domain.Entities.DoctorEntites;
using Domain.Exceptions;         
using Services.Abstractions;
using Shared.DoctorModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IMapper mapper;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
        {
            this.doctorRepository = doctorRepository;
            this.mapper = mapper;
        }

        public async Task<DoctorDTO?> GetDoctorByIdAsync(Guid doctorId)
        {
            var doctor = await doctorRepository.GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());
            return mapper.Map<DoctorDTO>(doctor);
        }

        public async Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            var doctors = await doctorRepository.GetDoctorsBySpecialtyAsync(specialty);
            return mapper.Map<IEnumerable<DoctorDTO>>(doctors);
        }

        public async Task<DoctorDTO> CreateDoctorAsync(DoctorDTO model)
        {
            var doctor = mapper.Map<Doctor>(model);
            doctor.Id = Guid.NewGuid();
            await doctorRepository.AddAsync(doctor);
            return mapper.Map<DoctorDTO>(doctor);
        }

        public async Task<DoctorDTO?> UpdateDoctorAsync(Guid doctorId, DoctorDTO model)
        {
            var doctor = await doctorRepository.GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());

            mapper.Map(model, doctor);
            doctorRepository.Update(doctor);
            return mapper.Map<DoctorDTO>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(Guid doctorId)
        {
            var doctor = await doctorRepository.GetAsync(doctorId);
            if (doctor == null)
                throw new DoctorNotFoundException(doctorId.ToString());

            doctorRepository.Delete(doctor);
            return true;
        }
    }
}
*/