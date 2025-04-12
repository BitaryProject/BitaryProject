/*using AutoMapper;
using Domain.Entities.MedicalRecordEntites;
using Domain.Contracts.NewModule;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.MedicalRecordModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

      //  public IMedicalRecordService MedicalRecordRepository { get; }
       
        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

     

        public async Task<MedicalRecordDTO?> GetMedicalRecordByIdAsync(Guid recordId)
        {
            var record = await _medicalRecordRepository.GetAsync(recordId);
            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());
            return _mapper.Map<MedicalRecordDTO>(record);
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetRecordsByPetIdAsync(Guid petId)
        {
            var records = await _medicalRecordRepository.GetRecordsByPetIdAsync(petId);
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordDTO model)
        {
            var dto = _mapper.Map<MedicalRecordDTO>(model);
            var record = _mapper.Map<MedicalRecord>(dto);
            record.Id = Guid.NewGuid();
            await _medicalRecordRepository.AddAsync(record);
            return _mapper.Map<MedicalRecordDTO>(record);
        }

        public async Task<MedicalRecordDTO?> UpdateMedicalRecordAsync(Guid recordId, MedicalRecordDTO model)
        {
            var record = await _medicalRecordRepository.GetAsync(recordId);
            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());
            _mapper.Map(model, record);
            _medicalRecordRepository.Update(record);
            return _mapper.Map<MedicalRecordDTO>(record);
        }

        public async Task<bool> DeleteMedicalRecordAsync(Guid recordId)
        {
            var record = await _medicalRecordRepository.GetAsync(recordId);
            if (record == null)
                throw new MedicalRecordNotFoundException(recordId.ToString());
            _medicalRecordRepository.Delete(record);
            return true;
        }

       
    }
}
*/