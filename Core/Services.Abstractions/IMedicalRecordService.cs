using Shared.MedicalRecordModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDTO?> GetMedicalRecordByIdAsync(Guid recordId);

        Task<IEnumerable<MedicalRecordDTO>> GetRecordsByPetIdAsync(Guid petId);

        Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordDTO model);

        Task<MedicalRecordDTO?> UpdateMedicalRecordAsync(Guid recordId, MedicalRecordDTO model);

        Task<bool> DeleteMedicalRecordAsync(Guid recordId);
    }
}
