using Core.Common.Specifications;

using Core.Domain.Contracts;

using Core.Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord, Guid>, IMedicalRecordRepository
    {
        private readonly StoreContext _context;
        
        public MedicalRecordRepository(StoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetIdAsync(Guid petProfileId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.PetProfileId == petProfileId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.RecordDate >= startDate && 
                       mr.RecordDate <= endDate)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDiagnosisAsync(string diagnosis)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.Diagnosis.Contains(diagnosis))
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<MedicalRecord> GetLatestMedicalRecordForPetAsync(Guid petProfileId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.PetProfileId == petProfileId)
                .OrderByDescending(mr => mr.RecordDate)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<MedicalRecord> MedicalRecords, int TotalCount)> GetPagedMedicalRecordsAsync(Core.Common.Specifications.Core.Common.Specifications.Core.Common.Specifications.ISpecification<MedicalRecord> specification, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specification, pageIndex, pageSize);
        }

        public async Task<bool> AddNoteToMedicalRecordAsync(Guid medicalRecordId, string note)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(medicalRecordId);
            if (medicalRecord == null)
                return false;

            medicalRecord.Notes = (medicalRecord.Notes ?? "") + "\n" + note;
            _context.MedicalRecords.Update(medicalRecord);
            return await _context.SaveChangesAsync() > 0;
        }
    }
    
    // Implementations for ISpecification methods
    

    

    

    

    } 








