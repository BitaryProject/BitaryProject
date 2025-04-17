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
    /// A mock implementation of IMedicalNoteRepository that returns sample data
    /// </summary>
    public class MockMedicalNoteRepository : IMedicalNoteRepository
    {
        private static readonly List<MedicalNote> _notes = new List<MedicalNote>
        {
            new MedicalNote
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa1"),
                Content = "Patient shows signs of improvement",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                MedicalRecordId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                DoctorId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb1")
            },
            new MedicalNote
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa2"),
                Content = "Prescribed medication for treatment",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                MedicalRecordId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                DoctorId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb1")
            },
            new MedicalNote
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa3"),
                Content = "Follow-up visit scheduled",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                MedicalRecordId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                DoctorId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb2")
            }
        };

        public Task<MedicalNote> GetByIdAsync(Guid id)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            return Task.FromResult(note);
        }

        public Task<IEnumerable<MedicalNote>> GetNotesByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            var notes = _notes.Where(n => n.MedicalRecordId == medicalRecordId);
            return Task.FromResult(notes);
        }

        public Task<IEnumerable<MedicalNote>> GetNotesByDoctorIdAsync(Guid doctorId)
        {
            var notes = _notes.Where(n => n.DoctorId == doctorId);
            return Task.FromResult(notes);
        }

        public Task<(IEnumerable<MedicalNote> Notes, int TotalCount)> GetPagedNotesAsync(
            ISpecification<MedicalNote> specification, int pageIndex, int pageSize)
        {
            var result = (_notes.AsEnumerable(), _notes.Count);
            return Task.FromResult(result);
        }

        public Task AddAsync(MedicalNote entity)
        {
            entity.Id = Guid.NewGuid();
            _notes.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(MedicalNote entity)
        {
            var index = _notes.FindIndex(n => n.Id == entity.Id);
            if (index >= 0)
            {
                _notes[index] = entity;
            }
        }

        public void Delete(MedicalNote entity)
        {
            _notes.RemoveAll(n => n.Id == entity.Id);
        }

        public Task<bool> AnyAsync()
        {
            return Task.FromResult(_notes.Any());
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_notes.Count);
        }
    }
} 