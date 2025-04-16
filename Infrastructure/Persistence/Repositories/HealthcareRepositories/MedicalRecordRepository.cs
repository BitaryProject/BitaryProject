using Domain.Contracts;
using Domain.Entities.HealthcareEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories.HealthcareRepositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord, Guid>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetProfileIdAsync(Guid petProfileId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.PetProfileId == petProfileId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.DoctorId == doctorId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(Guid petProfileId, DateTime startDate, DateTime endDate)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.PetProfile)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.PetProfileId == petProfileId && 
                       mr.RecordDate >= startDate && 
                       mr.RecordDate <= endDate)
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

        public async Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> GetPagedMedicalRecordsAsync(Specifications<MedicalRecord> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }
    }
} 