using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.HealthcareEntities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Services.Specifications;
using Shared.HealthcareModels;

namespace Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IHealthcareUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(
            IHealthcareUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MedicalRecordService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MedicalRecordDTO> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting medical record with ID {Id}", id);
            
            var spec = new MedicalRecordSpecification(id);
            var medicalRecord = await _unitOfWork.MedicalRecordRepository.FirstOrDefaultAsync(spec);
            
            if (medicalRecord == null)
                throw new NotFoundException($"Medical record with ID {id} not found");
                
            return _mapper.Map<MedicalRecordDTO>(medicalRecord);
        }

        public async Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByPetAsync(Guid petId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records for pet {PetId}", petId);
            
            var spec = new MedicalRecordSpecification(petId, pageIndex, pageSize);
            var records = await _unitOfWork.MedicalRecordRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.MedicalRecordRepository.CountAsync(spec);
            
            return new PagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(records),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records by doctor {DoctorId}", doctorId);
            
            var spec = new MedicalRecordSpecification(doctorId, true, pageIndex, pageSize);
            var records = await _unitOfWork.MedicalRecordRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.MedicalRecordRepository.CountAsync(spec);
            
            return new PagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(records),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records between {StartDate} and {EndDate}", startDate, endDate);
            
            var spec = new MedicalRecordSpecification(startDate, endDate, pageIndex, pageSize);
            var records = await _unitOfWork.MedicalRecordRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.MedicalRecordRepository.CountAsync(spec);
            
            return new PagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(records),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDiagnosisAsync(string diagnosis, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records by diagnosis containing {Diagnosis}", diagnosis);
            
            var spec = new MedicalRecordSpecification(diagnosis, pageIndex, pageSize);
            var records = await _unitOfWork.MedicalRecordRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.MedicalRecordRepository.CountAsync(spec);
            
            return new PagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(records),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordCreateDTO recordCreateDto)
        {
            _logger.LogInformation("Creating new medical record for pet {PetId} by doctor {DoctorId}", 
                recordCreateDto.PetId, recordCreateDto.DoctorId);
            
            // Verify that the doctor exists
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(recordCreateDto.DoctorId);
            if (doctor == null)
                throw new NotFoundException($"Doctor with ID {recordCreateDto.DoctorId} not found");
                
            // Verify that the pet exists
            var pet = await _unitOfWork.PetRepository.GetAsync(recordCreateDto.PetId);
            if (pet == null)
                throw new NotFoundException($"Pet with ID {recordCreateDto.PetId} not found");
                
            var medicalRecord = new MedicalRecord
            {
                RecordDate = recordCreateDto.RecordDate,
                Diagnosis = recordCreateDto.Diagnosis,
                Treatment = recordCreateDto.Treatment,
                Notes = recordCreateDto.Notes,
                AdditionalNotes = recordCreateDto.Notes, // For backward compatibility
                DoctorId = recordCreateDto.DoctorId,
                PetProfileId = recordCreateDto.PetId,
                PetId = recordCreateDto.PetId // For backward compatibility
            };
            
            await _unitOfWork.MedicalRecordRepository.AddAsync(medicalRecord);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Medical record {Id} created for pet {PetId} by doctor {DoctorId}", 
                medicalRecord.Id, medicalRecord.PetProfileId, medicalRecord.DoctorId);
                
            // Reload the entity with related data
            var spec = new MedicalRecordSpecification(medicalRecord.Id);
            var createdRecord = await _unitOfWork.MedicalRecordRepository.FirstOrDefaultAsync(spec);
            
            return _mapper.Map<MedicalRecordDTO>(createdRecord);
        }

        public async Task<MedicalRecordDTO> UpdateMedicalRecordAsync(Guid id, MedicalRecordUpdateDTO recordUpdateDto)
        {
            var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(id);
            if (medicalRecord == null)
                throw new NotFoundException($"Medical record with ID {id} not found");
                
            medicalRecord.RecordDate = recordUpdateDto.RecordDate;
            medicalRecord.Diagnosis = recordUpdateDto.Diagnosis;
            medicalRecord.Treatment = recordUpdateDto.Treatment;
            medicalRecord.Notes = recordUpdateDto.Notes;
            medicalRecord.AdditionalNotes = recordUpdateDto.Notes; // For backward compatibility
            
            _unitOfWork.MedicalRecordRepository.Update(medicalRecord);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Medical record {Id} updated", id);
            
            // Reload the entity with related data
            var spec = new MedicalRecordSpecification(id);
            var updatedRecord = await _unitOfWork.MedicalRecordRepository.FirstOrDefaultAsync(spec);
            
            return _mapper.Map<MedicalRecordDTO>(updatedRecord);
        }

        public async Task DeleteMedicalRecordAsync(Guid id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(id);
            if (medicalRecord == null)
                throw new NotFoundException($"Medical record with ID {id} not found");
                
            _unitOfWork.MedicalRecordRepository.Delete(medicalRecord);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Medical record {Id} deleted", id);
        }
    }
}
