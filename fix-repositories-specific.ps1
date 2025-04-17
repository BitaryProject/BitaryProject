# Fix Specific Repository Implementations
# This script addresses issues with specific repositories identified in the build output

# Fix MedicationRepository.cs
function Fix-MedicationRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\MedicationRepository.cs"
    if (Test-Path $repoPath) {
        $content = Get-Content $repoPath -Raw
        
        # First remove any problematic or incomplete method implementations
        $content = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Contracts;
using Core.Domain.Contracts.HealthcareContracts; 
using Core.Domain.Entities.HealthcareEntities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class MedicationRepository : GenericRepository<Medication, Guid>, IMedicationRepository
    {
        private readonly StoreContext _context;
        
        public MedicationRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IReadOnlyList<Medication>> GetMedicationsBySearchTermAsync(string searchTerm)
        {
            return await _context.Medications
                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) || 
                       p.Description.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
        }

        public async Task<Medication> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Medication> GetAsync(ISpecification<Medication> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<Medication> GetAsync(Expression<Func<Medication>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Medication>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IReadOnlyList<Medication>> GetAllAsync(ISpecification<Medication> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public async Task<IReadOnlyList<Medication>> GetAllWithSpecAsync(ISpecification<Medication> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public async Task<Medication> FindAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IReadOnlyList<Medication>> GetPagedAsync(ISpecification<Medication> specification, int pageIndex, int pageSize)
        {
            return await ApplySpecification(specification)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<Medication> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<bool> AnyAsync(ISpecification<Medication> specification)
        {
            return await ApplySpecification(specification).AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public new async Task<Medication> AddAsync(Medication entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<Medication> FirstOrDefaultAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IReadOnlyList<Medication>> ListAsync(ISpecification<Medication> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        private IQueryable<Medication> ApplySpecification(ISpecification<Medication> specification)
        {
            return SpecificationEvaluator<Medication>.GetQuery(_dbSet.AsQueryable(), specification);
        }
    }
}
"@
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed MedicationRepository.cs"
    }
}

# Fix ClinicRatingRepository.cs
function Fix-ClinicRatingRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\ClinicRatingRepository.cs"
    if (Test-Path $repoPath) {
        $content = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts.HealthcareContracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
    public class ClinicRatingRepository : GenericRepository<ClinicRating, Guid>, IClinicRatingRepository
    {
        private readonly StoreContext _context;
        
        public ClinicRatingRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
    }
}
"@
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed ClinicRatingRepository.cs"
    }
}

# Fix PetProfileRepository.cs with missing implementations
function Fix-PetProfileRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\PetProfileRepository.cs"
    if (Test-Path $repoPath) {
        $content = Get-Content $repoPath -Raw
        
        # Get the proper namespace from the existing file
        $namespace = [regex]::Match($content, "namespace\s+([\w\.]+)").Groups[1].Value
        
        # Rebuild the file with correct implementations
        $content = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts.HealthcareContracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace $namespace
{
    public class PetProfileRepository : GenericRepository<PetProfile, Guid>, IPetProfileRepository
    {
        private readonly StoreContext _context;
        
        public PetProfileRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IReadOnlyList<PetProfile>> GetPetProfilesByOwnerIdAsync(Guid ownerId)
        {
            return await _context.PetProfiles
                .Where(p => p.PetOwnerId == ownerId)
                .ToListAsync();
        }
        
        public async Task<ICollection<Guid>> GetPetIdsByOwnerIdAsync(Guid ownerId)
        {
            return await _context.PetProfiles
                .Where(p => p.PetOwnerId == ownerId)
                .Select(p => p.Id)
                .ToListAsync();
        }
        
        public async Task<IReadOnlyList<PetProfile>> GetPagedPetProfilesAsync(ISpecification<PetProfile> specification, int pageIndex, int pageSize)
        {
            return await ApplySpecification(specification)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        
        public async Task<IReadOnlyList<PetProfile>> GetPagedPetsAsync(ISpecification<PetProfile> specification, int pageIndex, int pageSize)
        {
            return await ApplySpecification(specification)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
"@
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed PetProfileRepository.cs"
    }
}

# Fix PetOwnerRepository.cs with missing implementations
function Fix-PetOwnerRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\PetOwnerRepository.cs"
    if (Test-Path $repoPath) {
        $content = Get-Content $repoPath -Raw
        
        # Get the proper namespace from the existing file
        $namespace = [regex]::Match($content, "namespace\s+([\w\.]+)").Groups[1].Value
        
        # Rebuild the file with correct implementations
        $content = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts.HealthcareContracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace $namespace
{
    public class PetOwnerRepository : GenericRepository<PetOwner, Guid>, IPetOwnerRepository
    {
        private readonly StoreContext _context;
        
        public PetOwnerRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<PetOwner> GetPetOwnerByUserIdAsync(string userId)
        {
            return await _context.PetOwners
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
        
        public async Task<IReadOnlyList<PetOwner>> GetPagedPetOwnersAsync(ISpecification<PetOwner> specification, int pageIndex, int pageSize)
        {
            return await ApplySpecification(specification)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
"@
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed PetOwnerRepository.cs"
    }
}

# Fix MedicalRecordRepository.cs with missing implementations
function Fix-MedicalRecordRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\MedicalRecordRepository.cs"
    if (Test-Path $repoPath) {
        $content = Get-Content $repoPath -Raw
        
        # Get the proper namespace from the existing file
        $namespace = [regex]::Match($content, "namespace\s+([\w\.]+)").Groups[1].Value
        
        # Rebuild the file with correct implementations
        $content = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts.HealthcareContracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace $namespace
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord, Guid>, IMedicalRecordRepository
    {
        private readonly StoreContext _context;
        
        public MedicalRecordRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IReadOnlyList<MedicalRecord>> GetMedicalRecordsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalRecords
                .Where(mr => mr.DoctorId == doctorId)
                .ToListAsync();
        }
        
        public async Task<IReadOnlyList<MedicalRecord>> GetPagedMedicalRecordsAsync(ISpecification<MedicalRecord> specification, int pageIndex, int pageSize)
        {
            return await ApplySpecification(specification)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
"@
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed MedicalRecordRepository.cs"
    }
}

# Fix PrescriptionRepository.cs with missing implementations
function Fix-PrescriptionRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\PrescriptionRepository.cs"
    if (Test-Path $repoPath) {
        $content = Get-Content $repoPath -Raw
        
        # Get the proper namespace from the existing file
        $namespace = [regex]::Match($content, "namespace\s+([\w\.]+)").Groups[1].Value
        
        # Rebuild the file with correct implementations
        $content = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts.HealthcareContracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace $namespace
{
    public class PrescriptionRepository : GenericRepository<Prescription, Guid>, IPrescriptionRepository
    {
        private readonly StoreContext _context;
        
        public PrescriptionRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IReadOnlyList<Prescription>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Prescriptions
                .Where(p => p.DoctorId == doctorId)
                .ToListAsync();
        }
        
        public async Task<IReadOnlyList<Prescription>> GetPagedPrescriptionsAsync(ISpecification<Prescription> specification, int pageIndex, int pageSize)
        {
            return await ApplySpecification(specification)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
"@
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed PrescriptionRepository.cs"
    }
}

# Fix StoreContext to include Healthcare entities
function Fix-StoreContext {
    $repoPath = "Infrastructure\Persistence\Data\StoreContext.cs"
    if (Test-Path $repoPath) {
        # Read the existing file
        $content = Get-Content $repoPath -Raw
        
        # Add missing using statements
        $usingStatements = @"
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Domain.Entities;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Entities.IdentityEntities;
using Core.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;

"@
        
        # Replace the existing using statements
        $content = $content -replace "(using.*\r\n)+", $usingStatements
        
        # Fix DbSet properties if needed
        if ($content -notmatch "public DbSet<PetOwner> PetOwners { get; set; }") {
            $insertPoint = $content.LastIndexOf("protected override void OnModelCreating")
            
            $dbSetProperties = @"
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<CustomerBasket> BasketItems { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        
        // Healthcare entities
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PetOwner> PetOwners { get; set; }
        public DbSet<PetProfile> PetProfiles { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<MedicalNote> MedicalNotes { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<DoctorRating> DoctorRatings { get; set; }
        public DbSet<ClinicRating> ClinicRatings { get; set; }
        
        "
            
            $content = $content.Insert($insertPoint, $dbSetProperties)
        }
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed StoreContext.cs"
    }
}

# Fix RatingRepository.cs
function Fix-RatingRepository {
    $repoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\RatingRepository.cs"
    if (Test-Path $repoPath) {
        $content = Get-Content $repoPath -Raw
        
        # Add missing using statements
        $usingStatements = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Contracts.HealthcareContracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

"@
        
        # Replace the existing using statements
        $content = $content -replace "(using.*\r\n)+", $usingStatements
        
        # Fix the interface declaration
        $content = $content -replace "IRatingRepository", "IRatingRepository<TEntity, TKey>"
        
        # Fix null returns to use null! for non-nullable reference types
        $content = $content -replace "return null;", "return null!;"
        
        Set-Content -Path $repoPath -Value $content
        Write-Host "Fixed RatingRepository.cs"
    }
}

# Call functions to fix specific repositories
Fix-MedicationRepository
Fix-ClinicRatingRepository
Fix-PetProfileRepository
Fix-PetOwnerRepository
Fix-MedicalRecordRepository
Fix-PrescriptionRepository
Fix-StoreContext
Fix-RatingRepository

Write-Host "Repository fixes complete." 