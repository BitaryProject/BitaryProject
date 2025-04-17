//using AutoMapper;
//using Domain.Contracts.NewModule; 
//using Core.Domain.Entities.DoctorEntites;
//using Domain.Exceptions;         
//using Services.Abstractions;
//using Shared.DoctorModels;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Services
//{
//    public class DoctorService : IDoctorService
//    {
//        private readonly IDoctorRepository doctorRepository;
//        private readonly IMapper mapper;
//      //  private IDoctorService doctorRepository1;

//        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
//        {
//            this.doctorRepository = doctorRepository;
//            this.mapper = mapper;
//        }

//      /*  public DoctorService(IDoctorService doctorRepository1, IMapper mapper)
//        {
//            this.doctorRepository1 = doctorRepository1;
//            this.mapper = mapper;
//        }
//      */
//        public async Task<DoctorDTO?> GetDoctorByIdAsync(int doctorId)
//        {
//            var doctor = await doctorRepository.GetAsync(doctorId);
//            if (doctor == null)
//                throw new DoctorNotFoundException(doctorId.ToString());
//            return mapper.Map<DoctorDTO>(doctor);
//        }

//        public async Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(string specialty)
//        {
//            var doctors = await doctorRepository.GetDoctorsBySpecialtyAsync(specialty);
//            return mapper.Map<IEnumerable<DoctorDTO>>(doctors);
//        }

//        public async Task<DoctorDTO> CreateDoctorAsync(DoctorDTO model)
//        {
//            var doctor = mapper.Map<Doctor>(model);
//            await doctorRepository.AddAsync(doctor);
//            return mapper.Map<DoctorDTO>(doctor);
//        }

//        public async Task<DoctorDTO?> UpdateDoctorAsync(int doctorId, DoctorDTO model)
//        {
//            var doctor = await doctorRepository.GetAsync(doctorId);
//            if (doctor == null)
//                throw new DoctorNotFoundException(doctorId.ToString());

//            mapper.Map(model, doctor);
//            doctorRepository.Update(doctor);
//            return mapper.Map<DoctorDTO>(doctor);
//        }

//        public async Task<bool> DeleteDoctorAsync(int doctorId)
//        {
//            var doctor = await doctorRepository.GetAsync(doctorId);
//            if (doctor == null)
//                throw new DoctorNotFoundException(doctorId.ToString());

//            doctorRepository.Delete(doctor);
//            return true;
//        }
//    }
//}
