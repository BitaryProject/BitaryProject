using AutoMapper;
using Domain.Entities.ClinicEntities;
using Domain.Contracts.NewModule; 
using Domain.Exceptions;          
using Services.Abstractions;
using Shared.ClinicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
  /*  public interface IClinicSearchService
    {
        Task<IEnumerable<ClinicDTO>> SearchClinicsAsync(string city, double? userLat, double? userLon, int radiusKm);
        Task<IEnumerable<ClinicDTO>> GetTopRatedClinicsAsync(int count);
    }
  */
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository clinicRepository;
        private readonly IMapper mapper;
      //  private IClinicService clinicRepository1;

        public ClinicService(IClinicRepository clinicRepository, IMapper mapper)
        {
            this.clinicRepository = clinicRepository;
            this.mapper = mapper;
        }

     /*   public ClinicService(IClinicService clinicRepository1, IMapper mapper)
        {
            this.clinicRepository1 = clinicRepository1;
            this.mapper = mapper;
        }
     */

        public async Task<ClinicDTO?> GetClinicByIdAsync(Guid clinicId)
        {
            var clinic = await clinicRepository.GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());
            return mapper.Map<ClinicDTO>(clinic);
        }

        public async Task<IEnumerable<ClinicDTO>> GetAllClinicsAsync()
        {
            var clinics = await clinicRepository.GetAllAsync();
            return mapper.Map<IEnumerable<ClinicDTO>>(clinics);
        }

        public async Task<ClinicDTO> CreateClinicAsync(ClinicDTO model)
        {
            var clinic = mapper.Map<Clinic>(model);
            clinic.Id = Guid.NewGuid(); 
            await clinicRepository.AddAsync(clinic);
            return mapper.Map<ClinicDTO>(clinic);
        }

        public async Task<ClinicDTO?> UpdateClinicAsync(Guid clinicId, ClinicDTO model)
        {
            var clinic = await clinicRepository.GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            mapper.Map(model, clinic);
            clinicRepository.Update(clinic);
            return mapper.Map<ClinicDTO>(clinic);
        }

        public async Task<bool> DeleteClinicAsync(Guid clinicId)
        {
            var clinic = await clinicRepository.GetAsync(clinicId);
            if (clinic == null)
                throw new ClinicNotFoundException(clinicId.ToString());

            clinicRepository.Delete(clinic); 
            return true;
        }
    }
}
