using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.MedicalRecordModels
{
    public record MedicalRecordCreateDTO
    {
        [Required(ErrorMessage = "Diagnosis is required")]
        public string Diagnosis { get; init; }

        public string Treatment { get; init; }

        [Required(ErrorMessage = "Record date is required")]
        public DateTime RecordDate { get; init; }

        public string Notes { get; init; }
    }
}