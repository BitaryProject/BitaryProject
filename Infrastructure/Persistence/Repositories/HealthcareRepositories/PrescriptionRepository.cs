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
    public class PrescriptionRepository : GenericRepository<Prescription, Guid>, IPrescriptionRepository
    {
        public PrescriptionRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPetProfileIdAsync(Guid petProfileId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.PetProfileId == petProfileId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.DoctorId == doctorId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsForPetAsync(Guid petProfileId)
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.PetProfileId == petProfileId && 
                       p.EndDate >= currentDate)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            return await _context.Prescriptions
                .Include(p => p.PetProfile)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Where(p => p.MedicalRecordId == medicalRecordId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetPagedPrescriptionsAsync(Specifications<Prescription> specifications, int pageIndex, int pageSize)
        {
            return await GetPagedAsync(specifications, pageIndex, pageSize);
        }
    }
} 