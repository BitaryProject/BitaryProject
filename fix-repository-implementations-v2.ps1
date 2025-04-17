# Fix Repository Implementations - Version 2
# This script addresses the following issues:
# 1. TEntity not found errors in repository implementations
# 2. Duplicate method declarations
# 3. Namespace issues with ISpecification
# 4. Missing implementations for repository interfaces

# Function to remove the duplicate implementation methods that are causing "namespace cannot directly contain members" errors
function Remove-DuplicateImplementations {
    param(
        [string]$FilePath
    )

    if (Test-Path $FilePath) {
        $content = Get-Content $FilePath -Raw
        
        # Remove the problematic implementation methods outside of class scope
        $pattern = '(?s)public\s+async\s+Task<TEntity[^>]*>\s+GetAsync\s*\(\s*ISpecification<TEntity>\s+specification\s*\)[^}]+}'
        $content = $content -replace $pattern, ''
        
        $pattern = '(?s)public\s+async\s+Task<IReadOnlyList<TEntity[^>]*>>\s+ListAsync\s*\(\s*ISpecification<TEntity>\s+specification\s*\)[^}]+}'
        $content = $content -replace $pattern, ''
        
        $pattern = '(?s)public\s+async\s+Task<int>\s+CountAsync\s*\(\s*ISpecification<TEntity>\s+specification\s*\)[^}]+}'
        $content = $content -replace $pattern, ''
        
        $pattern = '(?s)public\s+async\s+Task<bool>\s+AnyAsync\s*\(\s*ISpecification<TEntity>\s+specification\s*\)[^}]+}'
        $content = $content -replace $pattern, ''
        
        $pattern = '(?s)private\s+IQueryable<TEntity>\s+ApplySpecification\s*\(\s*ISpecification<TEntity>\s+specification\s*\)[^}]+}'
        $content = $content -replace $pattern, ''
        
        Set-Content -Path $FilePath -Value $content
        Write-Host "Removed duplicate implementations from $FilePath"
    }
}

# Function to replace ISpecification namespace references
function Fix-RepositoryImplementations {
    param(
        [string]$FilePath
    )

    if (Test-Path $FilePath) {
        $content = Get-Content $FilePath -Raw
        
        # Replace ISpecification references with fully qualified names
        $content = $content -replace "using Core\.Common\.Specifications;", ""
        $content = $content -replace "using Domain\.Contracts;", ""
        
        # Add a single using directive for the Core.Common.Specifications namespace
        if ($content -notmatch "using Core\.Common\.Specifications;") {
            $content = "using Core.Common.Specifications;`r`n" + $content
        }
        
        # Replace ISpecification<T> with Core.Common.Specifications.ISpecification<T>
        $content = $content -replace "ISpecification<([^>]+)>", "Core.Common.Specifications.ISpecification<`$1>"
        
        # Fix 'Domain.Entities' references
        $content = $content -replace "using Domain\.Entities;", "using Core.Domain.Entities;"
        $content = $content -replace "using Domain\.Entities\.HealthcareEntities;", "using Core.Domain.Entities.HealthcareEntities;"
        $content = $content -replace "using Domain\.Entities\.OrderEntities;", "using Core.Domain.Entities.OrderEntities;"
        
        # Remove 'using Specifications<T>;' which is invalid
        $content = $content -replace "using Specifications<T>;", ""
        $content = $content -replace "using Specifications;", ""
        
        Set-Content -Path $FilePath -Value $content
        Write-Host "Fixed repository implementation in: $FilePath"
    }
}

# Fix identity context and store context files
function Fix-IdentityContext {
    $identityContextPath = "Infrastructure\Persistence\Identity\IdentityContext.cs"
    if (Test-Path $identityContextPath) {
        $content = Get-Content $identityContextPath -Raw
        $content = $content -replace "using Domain\.Entities;", "using Core.Domain.Entities;"
        $content = $content -replace "Domain\.Entities", "Core.Domain.Entities"
        Set-Content -Path $identityContextPath -Value $content
        Write-Host "Fixed Identity Context"
    }
}

function Fix-StoreContext {
    $storeContextPath = "Infrastructure\Persistence\Data\StoreContext.cs"
    if (Test-Path $storeContextPath) {
        $content = Get-Content $storeContextPath -Raw
        $content = $content -replace "using Domain\.Entities;", "using Core.Domain.Entities;"
        $content = $content -replace "using Domain\.Entities\.HealthcareEntities;", "using Core.Domain.Entities.HealthcareEntities;"
        $content = $content -replace "using Domain\.Entities\.OrderEntities;", "using Core.Domain.Entities.OrderEntities;"
        $content = $content -replace "Domain\.Entities", "Core.Domain.Entities"
        Set-Content -Path $storeContextPath -Value $content
        Write-Host "Fixed Store Context"
    }
}

function Fix-OrderConfigurations {
    $orderConfigPath = "Infrastructure\Persistence\Data\Configurations\OrderConfigurations.cs"
    if (Test-Path $orderConfigPath) {
        $content = Get-Content $orderConfigPath -Raw
        $content = $content -replace "using Domain\.Entities;", "using Core.Domain.Entities;"
        $content = $content -replace "using Domain\.Entities\.OrderEntities;", "using Core.Domain.Entities.OrderEntities;"
        $content = $content -replace "Domain\.Entities", "Core.Domain.Entities"
        Set-Content -Path $orderConfigPath -Value $content
        Write-Host "Fixed Order Configurations"
    }
}

function Fix-RatingRepository {
    $ratingRepoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\RatingRepository.cs"
    if (Test-Path $ratingRepoPath) {
        $content = Get-Content $ratingRepoPath -Raw
        
        # Fix HealthcareDbContext not found error
        $content = $content -replace "using Core\.Domain\.Entities\.HealthcareEntities;", "using Core.Domain.Entities.HealthcareEntities;`r`nusing Infrastructure.Persistence.Data;"
        
        # Fix IRatingRepository not found error
        $content = $content -replace "using Core\.Domain\.Contracts\.HealthcareContracts;", "using Core.Domain.Contracts.HealthcareContracts;`r`nusing Core.Domain.Contracts.HealthcareContracts;"
        
        # Fix null literal to non-nullable reference type warnings
        $content = $content -replace "return null;", "return null!;"
        
        Set-Content -Path $ratingRepoPath -Value $content
        Write-Host "Fixed Rating Repository"
    }
}

function Fix-ClinicRatingRepository {
    $clinicRatingRepoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\ClinicRatingRepository.cs"
    if (Test-Path $clinicRatingRepoPath) {
        $content = Get-Content $clinicRatingRepoPath -Raw
        
        # Fix generic type arguments
        $content = $content -replace "GenericRepository<ClinicRating>", "GenericRepository<ClinicRating, Guid>"
        
        # Add implementation of interface methods
        $insertPoint = $content.LastIndexOf("}")
        $implementationTemplate = @"

    public async Task<ClinicRating> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<ClinicRating> GetAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync();
    }

    public async Task<ClinicRating> GetAsync(Expression<Func<ClinicRating>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<ClinicRating>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IReadOnlyList<ClinicRating>> GetAllAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return await ApplySpecification(specification).ToListAsync();
    }

    public async Task<IReadOnlyList<ClinicRating>> GetAllWithSpecAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return await ApplySpecification(specification).ToListAsync();
    }

    public async Task<ClinicRating> FindAsync(Expression<Func<ClinicRating, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<IReadOnlyList<ClinicRating>> GetPagedAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification, int pageIndex, int pageSize)
    {
        return await ApplySpecification(specification)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return await ApplySpecification(specification).CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<ClinicRating, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return await ApplySpecification(specification).AnyAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<ClinicRating, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<ClinicRating> AddAsync(ClinicRating entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public void Update(ClinicRating entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(ClinicRating entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<ClinicRating> FirstOrDefaultAsync(Expression<Func<ClinicRating, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<IReadOnlyList<ClinicRating>> ListAsync(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return await ApplySpecification(specification).ToListAsync();
    }

    private IQueryable<ClinicRating> ApplySpecification(Core.Common.Specifications.ISpecification<ClinicRating> specification)
    {
        return SpecificationEvaluator<ClinicRating>.GetQuery(_dbSet.AsQueryable(), specification);
    }
"@
        
        $content = $content.Insert($insertPoint, $implementationTemplate)
        Set-Content -Path $clinicRatingRepoPath -Value $content
        Write-Host "Fixed ClinicRating Repository"
    }
}

# Fix all repositories in HealthcareRepositories directory
$repositoriesDir = "Infrastructure\Persistence\Repositories"
Get-ChildItem -Path $repositoriesDir -Filter "*.cs" -Recurse | ForEach-Object {
    # First remove duplicate implementations that are causing namespace errors
    Remove-DuplicateImplementations -FilePath $_.FullName
    
    # Then fix the namespace and references
    Fix-RepositoryImplementations -FilePath $_.FullName
}

# Fix specific contexts and repositories
Fix-IdentityContext
Fix-StoreContext
Fix-OrderConfigurations
Fix-RatingRepository
Fix-ClinicRatingRepository

Write-Host "All fixes applied successfully" 