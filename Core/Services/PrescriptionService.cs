using Domain.Contracts;
using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Core.Services.Abstractions;
using Core.Services.Specifications;
using Core.Services.Specifications.Base;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Exceptions;

namespace Core.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IHealthcareUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrescriptionService(IHealthcareUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PrescriptionDTO> GetByIdAsync(Guid id)
        {
            var specification = new PrescriptionSpecification(id);
            var prescription = await _unitOfWork.PrescriptionRepository.FirstOrDefaultAsync(specification);
            
            if (prescription == null)
                throw new NotFoundException($"Prescription with ID {id} not found");

            return _mapper.Map<PrescriptionDTO>(prescription);
        }

        public async Task<PrescriptionDTO> GetByPrescriptionNumberAsync(string prescriptionNumber)
        {
            var specification = new PrescriptionSpecification(prescriptionNumber);
            var prescription = await _unitOfWork.PrescriptionRepository.FirstOrDefaultAsync(specification);
            
            if (prescription == null)
                throw new NotFoundException($"Prescription with number {prescriptionNumber} not found");

            return _mapper.Map<PrescriptionDTO>(prescription);
        }

        public async Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByDoctorAsync(Guid doctorId, int pageIndex, int pageSize)
        {
            var specification = new PrescriptionSpecification(doctorId, pageIndex, pageSize);
            var prescriptions = await _unitOfWork.PrescriptionRepository.ListAsync(specification);
            var count = await _unitOfWork.PrescriptionRepository.CountAsync(new PrescriptionSpecification(doctorId, 1, int.MaxValue));
            
            var dtos = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
            
            return new HealthcarePagedResultDTO<PrescriptionDTO>
            {
                Items = dtos.ToList(),
                TotalCount = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByPetAsync(Guid petId, int pageIndex, int pageSize)
        {
            var specification = new PrescriptionSpecification(petId, true, pageIndex, pageSize);
            var prescriptions = await _unitOfWork.PrescriptionRepository.ListAsync(specification);
            var count = await _unitOfWork.PrescriptionRepository.CountAsync(new PrescriptionSpecification(petId, true, 1, int.MaxValue));
            
            var dtos = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
            
            return new HealthcarePagedResultDTO<PrescriptionDTO>
            {
                Items = dtos.ToList(),
                TotalCount = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByStatusAsync(string status, int pageIndex, int pageSize)
        {
            var specification = new PrescriptionSpecification(status, pageIndex, pageSize);
            var prescriptions = await _unitOfWork.PrescriptionRepository.ListAsync(specification);
            var count = await _unitOfWork.PrescriptionRepository.CountAsync(new PrescriptionSpecification(status, 1, int.MaxValue));
            
            var dtos = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
            
            return new HealthcarePagedResultDTO<PrescriptionDTO>
            {
                Items = dtos.ToList(),
                TotalCount = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            var specification = new PrescriptionSpecification(startDate, endDate, pageIndex, pageSize);
            var prescriptions = await _unitOfWork.PrescriptionRepository.ListAsync(specification);
            var count = await _unitOfWork.PrescriptionRepository.CountAsync(new PrescriptionSpecification(startDate, endDate, 1, int.MaxValue));
            
            var dtos = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
            
            return new HealthcarePagedResultDTO<PrescriptionDTO>
            {
                Items = dtos.ToList(),
                TotalCount = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PrescriptionDTO> CreatePrescriptionAsync(PrescriptionCreateDTO prescriptionCreateDto)
        {
            // Validate entities exist
            var petProfile = await _unitOfWork.PetProfileRepository.GetAsync(prescriptionCreateDto.PatientId);
            if (petProfile == null)
                throw new NotFoundException($"Pet profile with ID {prescriptionCreateDto.PatientId} not found");
                
            var doctor = await _unitOfWork.DoctorRepository.GetAsync(prescriptionCreateDto.DoctorId);
            if (doctor == null)
                throw new NotFoundException($"Doctor with ID {prescriptionCreateDto.DoctorId} not found");
                
            if (prescriptionCreateDto.MedicalRecordId.HasValue)
            {
                var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(prescriptionCreateDto.MedicalRecordId.Value);
                if (medicalRecord == null)
                    throw new NotFoundException($"Medical record with ID {prescriptionCreateDto.MedicalRecordId} not found");
            }
            
            // Map the DTO to entity
            var prescription = _mapper.Map<Prescription>(prescriptionCreateDto);
            
            // Generate a unique prescription number
            prescription.PrescriptionNumber = $"RX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            // Process medication items if present
            if (prescriptionCreateDto.Medications != null && prescriptionCreateDto.Medications.Any())
            {
                prescription.MedicationItems = new List<PrescriptionMedicationItem>();
                
                foreach (var medicationDto in prescriptionCreateDto.Medications)
                {
                    // Find or create the medication
                    var medication = await _unitOfWork.MedicationRepository.FindByNameAsync(medicationDto.Name);
                    
                    if (medication == null)
                    {
                        // Create new medication
                        medication = new Medication
                        {
                            Name = medicationDto.Name,
                            Description = "Auto-created from prescription"
                        };
                        
                        await _unitOfWork.MedicationRepository.AddAsync(medication);
                        await _unitOfWork.SaveChangesAsync(); // Save to get the ID
                    }
                    
                    var medicationItem = _mapper.Map<PrescriptionMedicationItem>(medicationDto);
                    medicationItem.MedicationId = medication.Id;
                    
                    prescription.MedicationItems.Add(medicationItem);
                }
            }
            
            await _unitOfWork.PrescriptionRepository.AddAsync(prescription);
            await _unitOfWork.SaveChangesAsync();
            
            // Refresh the data to get all navigation properties
            return await GetByIdAsync(prescription.Id);
        }

        public async Task<PrescriptionDTO> UpdatePrescriptionAsync(Guid id, PrescriptionUpdateDTO prescriptionUpdateDto)
        {
            var prescription = await _unitOfWork.PrescriptionRepository.GetAsync(id);
            if (prescription == null)
                throw new NotFoundException($"Prescription with ID {id} not found");
                
            // Validate entities exist
            var petProfile = await _unitOfWork.PetProfileRepository.GetAsync(prescriptionUpdateDto.PetProfileId);
            if (petProfile == null)
                throw new NotFoundException($"Pet profile with ID {prescriptionUpdateDto.PetProfileId} not found");
                
            var doctor = await _unitOfWork.DoctorRepository.GetAsync(prescriptionUpdateDto.DoctorId);
            if (doctor == null)
                throw new NotFoundException($"Doctor with ID {prescriptionUpdateDto.DoctorId} not found");
                
            if (prescriptionUpdateDto.MedicalRecordId.HasValue)
            {
                var medicalRecord = await _unitOfWork.MedicalRecordRepository.GetAsync(prescriptionUpdateDto.MedicalRecordId.Value);
                if (medicalRecord == null)
                    throw new NotFoundException($"Medical record with ID {prescriptionUpdateDto.MedicalRecordId} not found");
            }
            
            // Update prescription fields
            _mapper.Map(prescriptionUpdateDto, prescription);
            
            // Process medication items if present
            if (prescriptionUpdateDto.Medications != null && prescriptionUpdateDto.Medications.Any())
            {
                // Initialize collection if null
                if (prescription.MedicationItems == null)
                    prescription.MedicationItems = new List<PrescriptionMedicationItem>();
                else
                {
                    // Remove existing items that aren't in the updated collection
                    var itemsToRemove = prescription.MedicationItems
                        .Where(item => !prescriptionUpdateDto.Medications.Any(m => m.Id.HasValue && m.Id.Value == item.Id))
                        .ToList();
                    
                    foreach (var item in itemsToRemove)
                    {
                        prescription.MedicationItems.Remove(item);
                    }
                }
                
                // Update existing or add new items
                foreach (var medicationDto in prescriptionUpdateDto.Medications)
                {
                    if (medicationDto.Id.HasValue)
                    {
                        // Update existing item
                        var existingItem = prescription.MedicationItems.FirstOrDefault(item => item.Id == medicationDto.Id.Value);
                        if (existingItem != null)
                        {
                            _mapper.Map(medicationDto, existingItem);
                            continue;
                        }
                    }
                    
                    // Find or create the medication
                    var medication = await _unitOfWork.MedicationRepository.FindByNameAsync(medicationDto.Name);
                    
                    if (medication == null)
                    {
                        // Create new medication
                        medication = new Medication
                        {
                            Name = medicationDto.Name,
                            Description = "Auto-created from prescription update"
                        };
                        
                        await _unitOfWork.MedicationRepository.AddAsync(medication);
                        await _unitOfWork.SaveChangesAsync(); // Save to get the ID
                    }
                    
                    var medicationItem = _mapper.Map<PrescriptionMedicationItem>(medicationDto);
                    medicationItem.MedicationId = medication.Id;
                    
                    prescription.MedicationItems.Add(medicationItem);
                }
            }
            
            _unitOfWork.PrescriptionRepository.Update(prescription);
            await _unitOfWork.SaveChangesAsync();
            
            // Refresh the data to get all navigation properties
            return await GetByIdAsync(prescription.Id);
        }

        public async Task<PrescriptionDTO> UpdatePrescriptionStatusAsync(Guid id, string status)
        {
            var prescription = await _unitOfWork.PrescriptionRepository.GetAsync(id);
            if (prescription == null)
                throw new NotFoundException($"Prescription with ID {id} not found");
            
            prescription.Status = status;
            
            _unitOfWork.PrescriptionRepository.Update(prescription);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetByIdAsync(prescription.Id);
        }

        public async Task DeletePrescriptionAsync(Guid id)
        {
            var prescription = await _unitOfWork.PrescriptionRepository.GetAsync(id);
            if (prescription == null)
                throw new NotFoundException($"Prescription with ID {id} not found");
                
            _unitOfWork.PrescriptionRepository.Delete(prescription);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsMedicationSafeForPetAsync(Guid petId, Guid medicationId)
        {
            // This would typically check for contraindications, allergies, etc.
            var pet = await _unitOfWork.PetProfileRepository.GetAsync(petId);
            if (pet == null)
                throw new NotFoundException($"Pet with ID {petId} not found");
                
            var medication = await _unitOfWork.MedicationRepository.GetAsync(medicationId);
            if (medication == null)
                throw new NotFoundException($"Medication with ID {medicationId} not found");
            
            // In a real implementation, you would check various factors like:
            // - Pet allergies
            // - Contraindications
            // - Weight-based dosing
            // - Species-specific concerns
            
            return true; // Placeholder implementation
        }

        // Older methods - kept for backwards compatibility

        public async Task<PrescriptionDTO> GetPrescriptionByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<PrescriptionDTO>> GetPrescriptionsByPetProfileIdAsync(Guid petProfileId)
        {
            var result = await GetPrescriptionsByPetAsync(petProfileId, 1, int.MaxValue);
            return result.Items;
        }

        public async Task<IEnumerable<PrescriptionDTO>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            var result = await GetPrescriptionsByDoctorAsync(doctorId, 1, int.MaxValue);
            return result.Items;
        }

        public async Task<IEnumerable<PrescriptionDTO>> GetActivePrescriptionsForPetAsync(Guid petProfileId)
        {
            var result = await GetPrescriptionsByStatusAsync("Active", 1, int.MaxValue);
            return result.Items.Where(p => p.PetId == petProfileId);
        }

        public async Task<IEnumerable<PrescriptionDTO>> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            var specification = new PrescriptionFilterSpecification(medicalRecordId);
            var prescriptions = await _unitOfWork.PrescriptionRepository.ListAsync(specification);
            return _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
        }

        public async Task<HealthcarePagedResultDTO<PrescriptionDTO>> GetPrescriptionsByMedicationAsync(string medicationName, int pageIndex, int pageSize)
        {
            // Create a specification to filter prescriptions containing the medication
            var specification = new PrescriptionFilterSpecification(medicationName);
            
            // Get prescriptions from repository
            var prescriptions = await _unitOfWork.PrescriptionRepository.ListAsync(specification);
            var count = await _unitOfWork.PrescriptionRepository.CountAsync(specification);
            
            var pagedPrescriptions = prescriptions
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // Map the prescriptions to DTOs
            var prescriptionDtos = _mapper.Map<IEnumerable<PrescriptionDTO>>(pagedPrescriptions);
            
            // Return paged result
            return new HealthcarePagedResultDTO<PrescriptionDTO>
            {
                Items = prescriptionDtos.ToList(),
                TotalCount = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }

    // Custom exception class
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
} 
