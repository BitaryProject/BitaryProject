using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.AppointmentEntities;
using Shared.MedicalRecordModels;
namespace Services.Abstractions
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(int recordId);
        
        Task<IEnumerable<MedicalRecordDTO>> GetRecordsByPetIdAsync(int petId);
        
        Task<IEnumerable<MedicalRecordDTO>> GetRecordsByDoctorIdAsync(int doctorId);
        
        Task<MedicalRecordDTO> GetRecordByAppointmentIdAsync(int appointmentId);
        
        Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordDTO model);
        
        Task<MedicalRecordDTO> UpdateMedicalRecordAsync(int recordId, MedicalRecordDTO model);
        
        Task<bool> DeleteMedicalRecordAsync(int recordId);
        Task<MedicalRecordDTO> CreateMedicalRecordForAppointmentAsync(int appointmentId, MedicalRecordCreateDTO model, int doctorId);
    }
}
