using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class Report
    {
        public string Title { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public string Summary { get; set; }
    }

    public class Statistics
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public double AverageAppointmentDuration { get; set; }
        public Dictionary<string, int> AppointmentsByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CommonDiagnoses { get; set; } = new Dictionary<string, int>();
    }

    public class CommonConditionsReportDTO
    {
        public List<ConditionStatistics> TopConditions { get; set; } = new List<ConditionStatistics>();
        public Dictionary<string, int> ConditionsBySpecies { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ConditionsByAge { get; set; } = new Dictionary<string, int>();
    }

    public class ConditionStatistics
    {
        public string ConditionName { get; set; }
        public int Occurrences { get; set; }
        public double PercentageOfTotal { get; set; }
    }

    public class TreatmentEffectivenessReportDTO
    {
        public List<TreatmentStatistics> Treatments { get; set; } = new List<TreatmentStatistics>();
        public Dictionary<string, double> EffectivenessByCondition { get; set; } = new Dictionary<string, double>();
    }

    public class TreatmentStatistics
    {
        public string TreatmentName { get; set; }
        public int TotalUsed { get; set; }
        public double SuccessRate { get; set; }
        public double AverageRecoveryTime { get; set; }
    }

    public class PetMedicalHistoryDTO
    {
        public Guid PetId { get; set; }
        public string PetName { get; set; }
        public List<MedicalRecordDTO> MedicalRecords { get; set; } = new List<MedicalRecordDTO>();
        public List<string> Allergies { get; set; } = new List<string>();
        public List<string> ChronicConditions { get; set; } = new List<string>();
    }

    public class PetMedicalReportDTO
    {
        public Guid PetId { get; set; }
        public string PetName { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public List<string> RecentConditions { get; set; } = new List<string>();
        public List<string> CurrentMedications { get; set; } = new List<string>();
        public string RecommendedFollowUp { get; set; }
    }
} 