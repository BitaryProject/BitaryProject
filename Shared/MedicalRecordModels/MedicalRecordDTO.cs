using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicalRecordModels
{
    public record MedicalRecordDTO
    {
        public Guid Id { get; init; }
        public string Diagnosis { get; init; }
        public string Treatment { get; init; }
        public DateTime RecordDate { get; init; }
        public string Notes { get; init; }
        public Guid PetId { get; init; }
        public Guid DoctorId { get; init; }
    }
}
