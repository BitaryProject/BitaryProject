using Core.Common.Specifications;

using Core.Domain.Contracts;

using Core.Domain.Entities.HealthcareEntities;

using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class MedicalNoteRepository : GenericRepository<MedicalNote, Guid>, IMedicalNoteRepository
    {
        private readonly StoreContext _context;
        
        public MedicalNoteRepository(StoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalNote>> GetNotesByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            return await _context.MedicalNotes
                .Include(mn => mn.Doctor)
                .Where(mn => mn.MedicalRecordId == medicalRecordId)
                .OrderByDescending(mn => mn.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalNote>> GetNotesByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalNotes
                .Include(mn => mn.MedicalRecord)
                .Where(mn => mn.DoctorId == doctorId)
                .OrderByDescending(mn => mn.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<MedicalNote> Notes, int TotalCount)> GetPagedNotesAsync(
            Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<MedicalNote> specification, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specification, pageIndex, pageSize);
        }
    }
    
    // Implementations for ISpecification methods
    

    

    

    

    } 








