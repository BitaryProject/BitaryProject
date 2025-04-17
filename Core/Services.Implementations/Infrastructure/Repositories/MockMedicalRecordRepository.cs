using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;

namespace Core.Services.Implementations.Infrastructure.Repositories
{
    /// <summary>
    /// A mock implementation of IMedicalRecordRepository that returns sample data
    /// </summary>
    public class MockMedicalRecordRepository : IMedicalRecordRepository
    {
        private static readonly List<MedicalRecord> _records = new List<MedicalRecord>
        {
            new MedicalRecord
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                RecordDate = DateTime.UtcNow.AddDays(-10),
                Diagnosis = "Respiratory infection",
                Treatment = "Antibiotics and rest",
                Notes = "Patient has been experiencing symptoms for a week",
                Symptoms = "Coughing, fever, fatigue",
                LabResults = "Elevated white blood cell count",
                Medications = "Amoxicillin 500mg BID for 10 days",
                PetProfileId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afc1"),
                DoctorId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb1"),
                MedicalNotes = new List<MedicalNote>()
            },
            new MedicalRecord
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                RecordDate = DateTime.UtcNow.AddDays(-5),
                Diagnosis = "Allergic dermatitis",
                Treatment = "Topical ointment and dietary changes",
                Notes = "Patient has seasonal allergies",
                Symptoms = "Itching, redness, hair loss",
                LabResults = "Normal blood work, skin culture negative",
                Medications = "Prednisone 10mg daily for 5 days",
                PetProfileId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afc2"),
                DoctorId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb2"),
                MedicalNotes = new List<MedicalNote>()
            }
        };

        public Task<MedicalRecord> GetByIdAsync(Guid id)
        {
            var record = _records.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(record);
        }

        public Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetIdAsync(Guid petId)
        {
            var records = _records.Where(r => r.PetProfileId == petId);
            return Task.FromResult(records);
        }

        public Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var records = _records.Where(r => r.RecordDate >= startDate && r.RecordDate <= endDate);
            return Task.FromResult(records);
        }

        public Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDiagnosisAsync(string diagnosis)
        {
            var records = _records.Where(r => r.Diagnosis.Contains(diagnosis, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(records);
        }

        public Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> GetPagedMedicalRecordsAsync(
            ISpecification<MedicalRecord> specification, int pageIndex, int pageSize)
        {
            var result = (_records.AsEnumerable(), _records.Count);
            return Task.FromResult(result);
        }

        public Task AddNoteToMedicalRecordAsync(Guid medicalRecordId, string note)
        {
            var record = _records.FirstOrDefault(r => r.Id == medicalRecordId);
            if (record != null)
            {
                if (string.IsNullOrEmpty(record.Notes))
                {
                    record.Notes = $"[{DateTime.UtcNow}] {note}";
                }
                else
                {
                    record.Notes += $"\n[{DateTime.UtcNow}] {note}";
                }
            }
            return Task.CompletedTask;
        }

        public Task AddAsync(MedicalRecord entity)
        {
            entity.Id = Guid.NewGuid();
            _records.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(MedicalRecord entity)
        {
            var index = _records.FindIndex(r => r.Id == entity.Id);
            if (index >= 0)
            {
                _records[index] = entity;
            }
        }

        public void Delete(MedicalRecord entity)
        {
            _records.RemoveAll(r => r.Id == entity.Id);
        }

        public Task<bool> AnyAsync()
        {
            return Task.FromResult(_records.Any());
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_records.Count);
        }
    }
} 