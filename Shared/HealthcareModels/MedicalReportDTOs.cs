using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class MedicalReportDTO
    {
        public Guid PetId { get; set; }
        public string PetName { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string OwnerName { get; set; }
        public DateTime ReportGeneratedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<MedicalRecordSummaryDTO> Records { get; set; }
        public IEnumerable<PrescriptionSummaryDTO> Prescriptions { get; set; }
        public IEnumerable<VaccinationDTO> Vaccinations { get; set; }
        public IEnumerable<ConditionSummaryDTO> ChronicConditions { get; set; }
        public IEnumerable<AllergyDTO> Allergies { get; set; }
        public VitalStatsSummaryDTO VitalStatistics { get; set; }
    }

    public class MedicalRecordSummaryDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
    }

    public class PrescriptionSummaryDTO
    {
        public Guid Id { get; set; }
        public DateTime IssuedDate { get; set; }
        public string DoctorName { get; set; }
        public string Medication { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class VaccinationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime AdministrationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string AdministeredBy { get; set; }
        public string Manufacturer { get; set; }
        public string BatchNumber { get; set; }
    }

    public class ConditionSummaryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DiagnosedDate { get; set; }
        public string DiagnosedBy { get; set; }
        public string CurrentStatus { get; set; }
        public string Notes { get; set; }
    }

    public class AllergyDTO
    {
        public Guid Id { get; set; }
        public string Allergen { get; set; }
        public string Severity { get; set; }
        public string Reaction { get; set; }
        public DateTime DiagnosedDate { get; set; }
        public string Notes { get; set; }
    }

    public class VitalStatsSummaryDTO
    {
        public double? AverageWeight { get; set; }
        public double? WeightChange { get; set; }
        public double? AverageTemperature { get; set; }
        public double? AverageHeartRate { get; set; }
        public double? AverageRespiratoryRate { get; set; }
        public IEnumerable<VitalStatsRecordDTO> Records { get; set; }
    }

    public class VitalStatsRecordDTO
    {
        public DateTime Date { get; set; }
        public double? Weight { get; set; }
        public double? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
    }

    public class DoctorActivityReportDTO
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TotalPatientsSeen { get; set; }
        public int NewPatients { get; set; }
        public int FollowUpVisits { get; set; }
        public int TotalPrescriptionsIssued { get; set; }
        public IEnumerable<CommonConditionDTO> TopTreatedConditions { get; set; }
        public double AverageAppointmentDuration { get; set; }
        public double AverageRating { get; set; }
    }

    public class ClinicActivityReportDTO
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public IEnumerable<DoctorSummaryDTO> TopDoctors { get; set; }
        public IEnumerable<CommonConditionDTO> TopTreatedConditions { get; set; }
        public double AverageAppointmentDuration { get; set; }
        public double AverageRating { get; set; }
        public double RevenueGenerated { get; set; }
    }

    public class DoctorSummaryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public int PatientsSeen { get; set; }
        public double AverageRating { get; set; }
    }

    public class CommonConditionDTO
    {
        public string Condition { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Species { get; set; }
        public IEnumerable<BreedStatisticDTO> BreedStatistics { get; set; }
    }

    public class BreedStatisticDTO
    {
        public string Breed { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class TreatmentEffectivenessDTO
    {
        public string Condition { get; set; }
        public string Treatment { get; set; }
        public int TotalCases { get; set; }
        public int SuccessfulCases { get; set; }
        public double SuccessRate { get; set; }
        public double AverageTreatmentDuration { get; set; }
        public IEnumerable<string> CommonSideEffects { get; set; }
    }

    public class HealthcareStatisticsDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalPatientsServed { get; set; }
        public int EmergencyCases { get; set; }
        public int RoutineCheckups { get; set; }
        public int SurgeriesPerformed { get; set; }
        public int VaccinationsAdministered { get; set; }
        public IEnumerable<SpeciesStatisticDTO> SpeciesStatistics { get; set; }
        public IEnumerable<CommonConditionDTO> TopDiagnoses { get; set; }
        public IEnumerable<MonthlyStatisticDTO> MonthlyTrends { get; set; }
    }

    public class SpeciesStatisticDTO
    {
        public string Species { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
        public IEnumerable<BreedStatisticDTO> BreedStatistics { get; set; }
    }

    public class MonthlyStatisticDTO
    {
        public DateTime Month { get; set; }
        public int TotalAppointments { get; set; }
        public int NewPatients { get; set; }
        public double Revenue { get; set; }
    }

    public class FileExportResultDTO
    {
        public byte[] FileContents { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
} 