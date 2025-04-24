using System.ComponentModel.DataAnnotations;

namespace Shared.MedicalRecordModels
{
    public record MedicalRecordUpdateDTO
    {
        [Required(ErrorMessage = "Diagnosis is required")]
        public string Diagnosis { get; init; }

        public string Treatment { get; init; }

        public string Notes { get; init; }
    }
} 