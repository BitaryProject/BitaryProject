using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Core.Services.Abstractions;
using Core.Services.Specifications;
using Shared.HealthcareModels;
using Core.Services.Specifications.Base;
using System.Linq.Expressions;
using Core.Common.Specifications;

namespace Core.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IHealthcareUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalRecordService> _logger;
        private readonly IMedicalNoteService _medicalNoteService;

        public MedicalRecordService(
            IHealthcareUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MedicalRecordService> logger,
            IMedicalNoteService medicalNoteService = null)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _medicalNoteService = medicalNoteService;
        }

        public async Task<MedicalRecordDTO> GetByIdAsync(Guid id)
        {
            return await GetMedicalRecordByIdAsync(id);
        }

        public async Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting medical record with ID {Id}", id);
            
            var spec = new MedicalRecordSpecification(id);
            var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(ConvertSpecification(spec));
            
            if (medicalRecord == null)
                throw new NotFoundException($"Medical record with ID {id} not found");
                
            return _mapper.Map<MedicalRecordDTO>(medicalRecord);
        }

        public async Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByPetAsync(Guid petId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records for pet {PetId}", petId);
            
            var spec = new MedicalRecordSpecification(petId, pageIndex, pageSize);
            var result = await _unitOfWork.MedicalRecordRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records by doctor {DoctorId}", doctorId);
            
            var spec = new MedicalRecordSpecification(doctorId, true, pageIndex, pageSize);
            var result = await _unitOfWork.MedicalRecordRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(result.Entities),
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records between {StartDate} and {EndDate}", startDate, endDate);
            
            var spec = new MedicalRecordSpecification(startDate, endDate, pageIndex, pageSize);
            var (records, totalCount) = await _unitOfWork.MedicalRecordRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<IEnumerable<MedicalRecordDTO>>(records).ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /*
        public async Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDiagnosisAsync(string diagnosis, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records by diagnosis containing {Diagnosis}", diagnosis);
            
            var spec = new MedicalRecordSpecification(diagnosis, pageIndex, pageSize);
            var records = await _unitOfWork.MedicalRecordRepository.GetAllWithSpecAsync(ConvertSpecification(spec));
            
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }
        */

        public async Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetMedicalRecordsByDiagnosisAsync(string diagnosis, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting medical records by diagnosis containing {Diagnosis}", diagnosis);
            
            var spec = new MedicalRecordSpecification(diagnosis, pageIndex, pageSize);
            var result = await _unitOfWork.MedicalRecordRepository.GetPagedAsync(ConvertSpecification(spec), pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                Items = _mapper.Map<List<MedicalRecordDTO>>(result.Entities),
                TotalCount = result.TotalCount,
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
                DoctorId = recordCreateDto.DoctorId,
                PetProfileId = recordCreateDto.PetId
            };
            
            await _unitOfWork.MedicalRecordRepository.AddAsync(medicalRecord);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Medical record {Id} created for pet {PetId} by doctor {DoctorId}", 
                medicalRecord.Id, medicalRecord.PetProfileId, medicalRecord.DoctorId);
                
            // Reload the entity with related data
            var spec = new MedicalRecordSpecification(medicalRecord.Id);
            var createdRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(ConvertSpecification(spec));
            
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
            
            _unitOfWork.MedicalRecordRepository.Update(medicalRecord);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Medical record {Id} updated", id);
            
            // Reload the entity with related data
            var spec = new MedicalRecordSpecification(medicalRecord.Id);
            var updatedRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(ConvertSpecification(spec));
            
            return _mapper.Map<MedicalRecordDTO>(updatedRecord);
        }

        public async Task<bool> DeleteMedicalRecordAsync(Guid id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(id);
            if (medicalRecord == null)
                throw new NotFoundException($"Medical record with ID {id} not found");
                
            _unitOfWork.MedicalRecordRepository.Delete(medicalRecord);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Medical record {Id} deleted", id);
            return true;
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetAllMedicalRecordsAsync()
        {
            var records = await _unitOfWork.MedicalRecordRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<HealthcarePagedResultDTO<MedicalRecordDTO>> GetPagedMedicalRecordsAsync(int pageIndex, int pageSize)
        {
            var spec = new MedicalRecordSpecification();
            var (records, totalCount) = await _unitOfWork.MedicalRecordRepository.GetPagedAsync(spec, pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = _mapper.Map<IEnumerable<MedicalRecordDTO>>(records)
            };
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByPetIdAsync(Guid petId)
        {
            var spec = new MedicalRecordSpecification(petId, pageIndex: 1, pageSize: int.MaxValue);
            var records = await _unitOfWork.MedicalRecordRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId)
        {
            var spec = new MedicalRecordSpecification(doctorId, true, 1, int.MaxValue);
            var records = await _unitOfWork.MedicalRecordRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<bool> AddNoteToMedicalRecordAsync(Guid recordId, string note)
        {
            var record = await _unitOfWork.MedicalRecordRepository.GetAsync(recordId);
            if (record == null) return false;

            // If the IMedicalNoteService is available, use it
            if (_medicalNoteService != null)
            {
                var noteDto = new MedicalNotesDTO
                {
                    MedicalRecordId = recordId,
                    Note = note,
                    Timestamp = DateTime.UtcNow
                };
                
                return await _medicalNoteService.AddNoteToMedicalRecordAsync(noteDto);
            }
            
            // Fallback to the original implementation
            record.Notes += $"\n{DateTime.UtcNow:g}: {note}";
            _unitOfWork.MedicalRecordRepository.Update(record);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> ExportPetMedicalHistoryAsync(Guid petId)
        {
            _logger.LogInformation("Exporting medical history for pet with ID: {PetId}", petId);
            
            // Get all medical records for the pet
            var medicalRecords = await _unitOfWork.MedicalRecordRepository.ListAsync(
                ConvertSpecification(new MedicalRecordSpecification(petId)));
            
            var medicalRecordDtos = _mapper.Map<IEnumerable<MedicalRecordDTO>>(medicalRecords);
            
            // TODO: Implement export functionality to generate PDF/Excel file
            // For now, just return a placeholder byte array
            return new byte[0]; // This will be replaced with actual PDF/Excel generation
        }

        public async Task<Report> GeneratePetMedicalReportAsync(Guid petId)
        {
            var records = await GetMedicalRecordsByPetIdAsync(petId);
            // Implementation for generating report would go here
            throw new NotImplementedException("Report generation not yet implemented");
        }

        public async Task<Report> GenerateDoctorActivityReportAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            var spec = new MedicalRecordSpecification(startDate, endDate, 1, int.MaxValue);
            var records = await _unitOfWork.MedicalRecordRepository.GetAllWithSpecAsync(spec);
            var doctorRecords = records.Where(r => r.DoctorId == doctorId);
            // Implementation for generating report would go here
            throw new NotImplementedException("Report generation not yet implemented");
        }

        public async Task<Report> GenerateClinicActivityReportAsync(Guid clinicId, DateTime startDate, DateTime endDate)
        {
            // Implementation for generating clinic activity report would go here
            throw new NotImplementedException("Report generation not yet implemented");
        }

        public async Task<Statistics> GetHealthcareStatisticsAsync()
        {
            // Implementation for generating healthcare statistics would go here
            throw new NotImplementedException("Statistics generation not yet implemented");
        }

        public async Task<Report> GetCommonConditionsReportAsync()
        {
            // Implementation for generating common conditions report would go here
            throw new NotImplementedException("Report generation not yet implemented");
        }

        public async Task<Report> GetTreatmentEffectivenessReportAsync()
        {
            // Implementation for generating treatment effectiveness report would go here
            throw new NotImplementedException("Report generation not yet implemented");
        }

        public async Task<HealthcarePagedResultDTO<MedicalRecordDTO>> SearchMedicalRecordsAsync(string searchTerm, int pageIndex, int pageSize)
        {
            var spec = new MedicalRecordSpecification(m => 
                m.Diagnosis.Contains(searchTerm) || 
                m.Treatment.Contains(searchTerm) || 
                m.Notes.Contains(searchTerm));
                
            var (records, totalCount) = await _unitOfWork.MedicalRecordRepository.GetPagedAsync(spec, pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = _mapper.Map<IEnumerable<MedicalRecordDTO>>(records)
            };
        }

        public async Task<PetMedicalHistoryDTO> GetPetMedicalHistoryAsync(Guid petId)
        {
            var pet = await _unitOfWork.PetRepository.GetAsync(petId);
            if (pet == null)
                throw new NotFoundException($"Pet with ID {petId} not found");
                
            var records = await GetMedicalRecordsByPetIdAsync(petId);
            
            var history = new PetMedicalHistoryDTO
            {
                PetId = petId,
                PetName = pet.Name,
                MedicalRecords = records.ToList(),
                Allergies = await GetPetAllergiesAsync(petId),
                ChronicConditions = await GetPetChronicConditionsAsync(petId)
            };
            
            return history;
        }

        public async Task<PetMedicalReportDTO> GetPetMedicalReportAsync(Guid petId)
        {
            var pet = await _unitOfWork.PetRepository.GetAsync(petId);
            if (pet == null)
                throw new NotFoundException($"Pet with ID {petId} not found");
                
            var recentRecords = await GetRecentMedicalRecordsAsync(petId, 5);
            
            var report = new PetMedicalReportDTO
            {
                PetId = petId,
                PetName = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Age = CalculatePetAge(pet.DateOfBirth),
                Weight = pet.Weight,
                RecentConditions = ExtractRecentConditions(recentRecords),
                CurrentMedications = await GetCurrentMedicationsAsync(petId),
                RecommendedFollowUp = DetermineRecommendedFollowUp(recentRecords)
            };
            
            return report;
        }

        private async Task<List<string>> GetPetAllergiesAsync(Guid petId)
        {
            // Implementation would depend on how allergies are stored
            // This is a placeholder
            return new List<string>();
        }

        private async Task<List<string>> GetPetChronicConditionsAsync(Guid petId)
        {
            // Implementation would depend on how chronic conditions are stored
            // This is a placeholder
            return new List<string>();
        }

        private async Task<List<MedicalRecordDTO>> GetRecentMedicalRecordsAsync(Guid petId, int count)
        {
            var spec = new MedicalRecordSpecification(petId, 1, count);
            var records = await _unitOfWork.MedicalRecordRepository.GetAllWithSpecAsync(ConvertSpecification(spec));
            return _mapper.Map<List<MedicalRecordDTO>>(records);
        }

        private List<string> ExtractRecentConditions(List<MedicalRecordDTO> records)
        {
            return records.Select(r => r.Diagnosis).Distinct().ToList();
        }

        private async Task<List<string>> GetCurrentMedicationsAsync(Guid petId)
        {
            // Implementation would depend on how medications are stored
            // This is a placeholder
            return new List<string>();
        }

        private string DetermineRecommendedFollowUp(List<MedicalRecordDTO> records)
        {
            // Logic to determine follow-up recommendation based on recent medical records
            // This is a simplified placeholder
            if (!records.Any())
                return "No recent medical records.";
            
            var mostRecent = records.OrderByDescending(r => r.RecordDate).First();
            var daysSinceLastVisit = (DateTime.Now - mostRecent.RecordDate).TotalDays;
            
            if (daysSinceLastVisit < 7)
                return "Follow up in 1 week.";
            else if (daysSinceLastVisit < 30)
                return "Follow up in 1 month.";
            else
                return "Schedule a routine checkup.";
        }

        private int CalculatePetAge(DateTime dateOfBirth)
        {
            return (int)((DateTime.Now - dateOfBirth).TotalDays / 365.25);
        }

        private Core.Common.Specifications.ISpecification<MedicalRecord> ConvertSpecification(MedicalRecordSpecification spec)
        {
            // Create a custom specification adapter that implements the service specification interface
            var customSpec = new CustomMedicalRecordSpecification(spec.Criteria);
            
            // Copy over all the properties/settings
            foreach (var include in spec.Includes)
                customSpec.AddInclude(include);
            
            foreach (var includeString in spec.IncludeStrings)
                customSpec.AddInclude(includeString);
            
            if (spec.OrderBy != null)
                customSpec.AddOrderBy(spec.OrderBy);
            
            if (spec.OrderByDescending != null)
                customSpec.ApplyOrderByDescending(spec.OrderByDescending);
            
            if (spec.IsPagingEnabled)
                customSpec.ApplyPaging(spec.Skip, spec.Take);
            
            return customSpec;
        }

        public async Task<MedicalNoteDTO> GetMedicalNoteByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting medical note with ID {Id}", id);
            
            var spec = new MedicalNoteSpecification(id);
            var note = await _unitOfWork.MedicalNoteRepository.GetAsync(spec);
            
            if (note == null)
                throw new NotFoundException($"Medical note with ID {id} not found");
                
            return _mapper.Map<MedicalNoteDTO>(note);
        }

        public async Task<IEnumerable<MedicalNoteDTO>> GetMedicalNotesByRecordIdAsync(Guid recordId)
        {
            _logger.LogInformation("Getting medical notes for record ID {RecordId}", recordId);
            
            var notes = await _unitOfWork.MedicalNoteRepository.GetNotesByMedicalRecordIdAsync(recordId);
            return _mapper.Map<IEnumerable<MedicalNoteDTO>>(notes);
        }

        public async Task<HealthcarePagedResultDTO<MedicalNoteDTO>> GetPagedMedicalNotesByRecordIdAsync(Guid recordId, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Getting paged medical notes for record ID {RecordId}. Page: {PageIndex}, Size: {PageSize}", 
                recordId, pageIndex, pageSize);
            
            var spec = new MedicalNoteSpecification(recordId, pageIndex, pageSize);
            var (notes, totalCount) = await _unitOfWork.MedicalNoteRepository.GetPagedNotesAsync(spec, pageIndex, pageSize);
            
            return new HealthcarePagedResultDTO<MedicalNoteDTO>
            {
                Items = _mapper.Map<IEnumerable<MedicalNoteDTO>>(notes),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<MedicalNoteDTO> CreateMedicalNoteAsync(MedicalNoteCreateDTO noteDto)
        {
            _logger.LogInformation("Creating medical note for record ID {RecordId}", noteDto.MedicalRecordId);
            
            // Verify medical record exists
            var record = await _unitOfWork.MedicalRecordRepository.GetByIdAsync(noteDto.MedicalRecordId);
            if (record == null)
                throw new NotFoundException($"Medical record with ID {noteDto.MedicalRecordId} not found");
            
            // Get current user as doctor
            var doctorId = record.DoctorId; // In a real implementation, get from current user
            
            var note = new MedicalNote
            {
                Content = noteDto.Content,
                CreatedAt = DateTime.UtcNow,
                MedicalRecordId = noteDto.MedicalRecordId,
                DoctorId = doctorId
            };
            
            await _unitOfWork.MedicalNoteRepository.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetMedicalNoteByIdAsync(note.Id);
        }

        public async Task<MedicalNoteDTO> UpdateMedicalNoteAsync(Guid id, MedicalNoteUpdateDTO noteDto)
        {
            _logger.LogInformation("Updating medical note with ID {Id}", id);
            
            var note = await _unitOfWork.MedicalNoteRepository.GetByIdAsync(id);
            if (note == null)
                throw new NotFoundException($"Medical note with ID {id} not found");
            
            // In a real implementation, verify the current user has permission to edit this note
            
            note.Content = noteDto.Content;
            
            _unitOfWork.MedicalNoteRepository.Update(note);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetMedicalNoteByIdAsync(id);
        }

        public async Task<bool> DeleteMedicalNoteAsync(Guid id)
        {
            _logger.LogInformation("Deleting medical note with ID {Id}", id);
            
            var note = await _unitOfWork.MedicalNoteRepository.GetByIdAsync(id);
            if (note == null)
                return false;
            
            // In a real implementation, verify the current user has permission to delete this note
            
            _unitOfWork.MedicalNoteRepository.Delete(note);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        // Fix the method to use HealthcarePagedResultDTO
        private HealthcarePagedResultDTO<MedicalRecordDTO> ConvertToPagedResult((IEnumerable<MedicalRecord> Items, int TotalCount) result, int pageIndex, int pageSize)
        {
            var medicalRecordDtos = _mapper.Map<IEnumerable<MedicalRecordDTO>>(result.Items).ToList();
            
            return new HealthcarePagedResultDTO<MedicalRecordDTO>
            {
                Items = medicalRecordDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<ICollection<MedicalRecordDTO>> GetPetMedicalHistoryRecordsAsync(Guid petId)
        {
            _logger.LogInformation("Retrieving complete medical history records for pet with ID: {PetId}", petId);
            
            var spec = new MedicalRecordSpecification(petId);
            
            var records = await _unitOfWork.MedicalRecordRepository.ListAsync(ConvertSpecification(spec));
            var medicalRecordDtos = _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
            
            return ConvertToCollection(medicalRecordDtos);
        }

        // Add back the helper method for converting to ICollection
        private ICollection<MedicalRecordDTO> ConvertToCollection(IEnumerable<MedicalRecordDTO> items)
        {
            return items.ToList();
        }

        // Helper method to convert between paged result formats
        // This helps bridge the gap between the service and controller layer DTOs
        private PagedResultDTO<T> ConvertToPagedResultDTO<T>(HealthcarePagedResultDTO<T> healthcareResult)
        {
            if (healthcareResult == null)
                return new PagedResultDTO<T>();
                
            return new PagedResultDTO<T>
            {
                Items = healthcareResult.Items,
                TotalCount = healthcareResult.TotalCount,
                PageIndex = healthcareResult.PageIndex,
                PageSize = healthcareResult.PageSize
            };
        }
    }

    // Custom implementation to bridge the gap between BaseSpecification and Specifications
    public class CustomMedicalRecordSpecification : Core.Common.Specifications.ISpecification<MedicalRecord>
    {
        private readonly Expression<Func<MedicalRecord, bool>> _criteria;
        private readonly List<Expression<Func<MedicalRecord, object>>> _includes = new();
        private readonly List<string> _includeStrings = new();
        private Expression<Func<MedicalRecord, object>> _orderBy;
        private Expression<Func<MedicalRecord, object>> _orderByDescending;
        private Expression<Func<MedicalRecord, object>> _groupBy;
        private int _take;
        private int _skip;
        private bool _isPagingEnabled;

        public CustomMedicalRecordSpecification(Expression<Func<MedicalRecord, bool>> criteria)
        {
            _criteria = criteria;
        }

        public Expression<Func<MedicalRecord, bool>> Criteria => _criteria;
        public List<Expression<Func<MedicalRecord, object>>> Includes => _includes;
        public List<string> IncludeStrings => _includeStrings;
        public Expression<Func<MedicalRecord, object>> OrderBy => _orderBy;
        public Expression<Func<MedicalRecord, object>> OrderByDescending => _orderByDescending;
        public Expression<Func<MedicalRecord, object>> GroupBy => _groupBy;
        public int Take => _take;
        public int Skip => _skip;
        public bool IsPagingEnabled => _isPagingEnabled;

        public void AddCriteria(Expression<Func<MedicalRecord, bool>> criteria)
        {
            // Not implemented in this context
        }

        public void AddInclude(Expression<Func<MedicalRecord, object>> includeExpression)
        {
            _includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            _includeStrings.Add(includeString);
        }

        public void AddOrderBy(Expression<Func<MedicalRecord, object>> orderByExpression)
        {
            _orderBy = orderByExpression;
        }

        public void ApplyOrderByDescending(Expression<Func<MedicalRecord, object>> orderByDescExpression)
        {
            _orderByDescending = orderByDescExpression;
        }

        public void ApplyPaging(int skip, int take)
        {
            _skip = skip;
            _take = take;
            _isPagingEnabled = true;
        }
    }
}
