using AutoMapper;
using Domain.Entities.HealthcareEntities;
using Shared.HealthcareModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.MappingProfiles
{
    public class HealthcarePrescriptionProfile : Profile
    {
        public HealthcarePrescriptionProfile()
        {
            // Prescription to DTO mapping
            CreateMap<Prescription, PrescriptionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PrescriptionNumber, opt => opt.MapFrom(src => src.PrescriptionNumber))
                .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => src.IssuedDate))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetProfile != null ? src.PetProfile.PetName : null))
                .ForMember(dest => dest.PetId, opt => opt.MapFrom(src => src.PetId))
                .ForMember(dest => dest.PetOwnerName, opt => opt.Ignore()) // Will be set manually if needed
                .ForMember(dest => dest.MedicationItems, opt => opt.MapFrom(src => src.MedicationItems));

            // DTO to Prescription mapping
            CreateMap<PrescriptionDTO, Prescription>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PrescriptionNumber, opt => opt.MapFrom(src => src.PrescriptionNumber))
                .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => src.IssuedDate))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.PetId, opt => opt.MapFrom(src => src.PetId))
                .ForMember(dest => dest.MedicationItems, opt => opt.MapFrom(src => src.MedicationItems))
                .ForMember(dest => dest.PrescriptionDate, opt => opt.MapFrom(src => src.IssuedDate))
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.PetProfile, opt => opt.Ignore());

            // PrescriptionCreateDTO to Prescription mapping
            CreateMap<PrescriptionCreateDTO, Prescription>()
                .ForMember(dest => dest.PrescriptionDate, opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.PetProfileId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.PetId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.MedicalRecordId, opt => opt.MapFrom(src => src.MedicalRecordId))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.MedicationItems, opt => opt.Ignore())
                .ForMember(dest => dest.PrescriptionNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.PetProfile, opt => opt.Ignore());

            // PrescriptionUpdateDTO to Prescription mapping
            CreateMap<PrescriptionUpdateDTO, Prescription>()
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Medication, opt => opt.MapFrom(src => src.Medication))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.PetProfileId, opt => opt.MapFrom(src => src.PetProfileId))
                .ForMember(dest => dest.PetId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.MedicalRecordId, opt => opt.MapFrom(src => src.MedicalRecordId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.PetProfile, opt => opt.Ignore())
                .ForMember(dest => dest.MedicationItems, opt => opt.Ignore());

            // PrescriptionCreateUpdateDTO to Prescription mapping
            CreateMap<PrescriptionCreateUpdateDTO, Prescription>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Medication, opt => opt.MapFrom(src => src.Medication))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.PetProfileId, opt => opt.MapFrom(src => src.PetProfileId))
                .ForMember(dest => dest.PetId, opt => opt.MapFrom(src => src.PetProfileId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.MedicalRecordId, opt => opt.MapFrom(src => src.MedicalRecordId))
                .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PrescriptionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.PrescriptionNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.PetProfile, opt => opt.Ignore())
                .ForMember(dest => dest.MedicationItems, opt => opt.Ignore());

            // Medication mappings
            CreateMap<Medication, MedicationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DosageForm, opt => opt.MapFrom(src => src.DosageForm))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<MedicationDTO, Medication>();

            // Prescription medication item mappings
            CreateMap<PrescriptionMedicationItem, PrescriptionMedicationItemDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MedicationId, opt => opt.MapFrom(src => src.MedicationId))
                .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src => src.Medication != null ? src.Medication.Name : null))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions));

            CreateMap<PrescriptionMedicationItemDTO, PrescriptionMedicationItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MedicationId, opt => opt.MapFrom(src => src.MedicationId))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Medication, opt => opt.Ignore());

            // PrescriptionMedicationCreateDTO to PrescriptionMedicationItem mapping
            CreateMap<PrescriptionMedicationCreateDTO, PrescriptionMedicationItem>()
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.MedicationId, opt => opt.Ignore())
                .ForMember(dest => dest.Medication, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => 1)); // Default quantity

            // PrescriptionMedicationUpdateDTO to PrescriptionMedicationItem mapping
            CreateMap<PrescriptionMedicationUpdateDTO, PrescriptionMedicationItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? Guid.NewGuid()))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.MedicationId, opt => opt.Ignore())
                .ForMember(dest => dest.Medication, opt => opt.Ignore());

            // Pagination mappings
            CreateMap<Domain.Entities.HealthcareEntities.PagedResult<Prescription>, Shared.HealthcareModels.PagedResultDTO<PrescriptionDTO>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount))
                .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));
        }
    }
} 