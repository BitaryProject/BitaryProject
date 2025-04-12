/*using Domain.Contracts.NewModule;
using Domain.Entities.ClinicEntities;
using Shared.ClinicModels;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ClinicSearchService : IClinicSearchService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;
s
        public ClinicSearchService(
            IClinicRepository clinicRepository,
            IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClinicDTO>> GetTopRatedClinicsAsync(int count)
        {
            var clinics = await _clinicRepository.GetTopRatedClinicsAsync(count);
            return _mapper.Map<IEnumerable<ClinicDTO>>(clinics);
        }

        public async Task<IEnumerable<ClinicDTO>> SearchClinicsAsync(string city)
        {
            var clinics = await _clinicRepository.GetClinicsByCityAsync(city);
            return _mapper.Map<IEnumerable<ClinicDTO>>(clinics);
        }

        public async Task<IEnumerable<ClinicDTO>> SearchClinicsAsync(string city, int radiusKm)
        {
            var clinics = await _clinicRepository.SearchClinicsInRadiusAsync(city, radiusKm);
            return _mapper.Map<IEnumerable<ClinicDTO>>(clinics);
        }

        public Task<IEnumerable<ClinicDTO>> SearchClinicsAsync(string city, double? userLat, double? userLon, int radiusKm)
        {
            throw new NotImplementedException();
        }
    }
}*/