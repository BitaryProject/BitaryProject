using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class MedicalRecordDTO
    {
        public Guid Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public Guid PetId { get; set; }
        public string PetName { get; set; }
    }

    public class MedicalRecordCreateDTO
    {
        [Required]
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Diagnosis { get; set; }

        [Required]
        public string Treatment { get; set; }

        public string Notes { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid PetId { get; set; }
    }

    public class MedicalRecordUpdateDTO
    {
        [Required]
        public DateTime RecordDate { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        [Required]
        public string Treatment { get; set; }

        public string Notes { get; set; }
    }
} 