using Core.Domain.Contracts;
using Core.Domain.Entities.HealthcareEntities;
using Core.Common.Specifications;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class MedicalNoteRepository : BaseHealthcareRepository<MedicalNote, Guid>, IMedicalNoteRepository
    {
        public MedicalNoteRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicalNote>> GetNotesByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            return await _storeContext.MedicalNotes
                .Include(mn => mn.Doctor)
                .Where(mn => mn.MedicalRecordId == medicalRecordId)
                .OrderByDescending(mn => mn.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalNote>> GetNotesByDoctorIdAsync(Guid doctorId)
        {
            return await _storeContext.MedicalNotes
                .Include(mn => mn.MedicalRecord)
                .Where(mn => mn.DoctorId == doctorId)
                .OrderByDescending(mn => mn.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<MedicalNote> Notes, int TotalCount)> GetPagedNotesAsync(
            Core.Common.Specifications.ISpecification<MedicalNote> specification, int pageIndex, int pageSize)
        {
            var query = SpecificationEvaluator<MedicalNote>.GetQuery(_storeContext.MedicalNotes.AsQueryable(), specification);
            
            var totalCount = await query.CountAsync();
            var notes = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return (notes, totalCount);
        }
    }
} 