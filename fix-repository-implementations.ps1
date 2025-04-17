$repoDir = "Infrastructure\Persistence\Repositories"

# Function to replace ISpecification references in repository files
function Fix-RepositoryImplementations {
    param (
        [string]$filePath
    )

    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Make sure we're using the right ISpecification
        $content = $content -replace "using Core\.Domain\.Contracts;", "using Core.Domain.Contracts;`nusing Core.Common.Specifications;"
        
        # Add missing interface implementation methods for ISpecification
        if ($content -match "class\s+(\w+Repository)\s*:\s*(\w+)Repository" -or 
            $content -match "class\s+(\w+Repository)\s*:\s*GenericRepository") {
            
            # Check if the file is missing ISpecification implementation methods
            if ($content -match "ISpecification<" -and 
                -not ($content -match "GetAsync\(ISpecification<" -or 
                      $content -match "ListAsync\(ISpecification<")) {
                
                Write-Host "Fixing repository implementation in: $filePath"
                
                # Find where to insert the implementation
                $classEndPos = $content.LastIndexOf('}')
                if ($classEndPos -gt 0) {
                    $implText = @"
    
    // Implementations for ISpecification methods
    public async Task<TEntity> GetAsync(ISpecification<TEntity> specification)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification)
    {
        return await ApplySpecification(specification).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> specification)
    {
        return await ApplySpecification(specification).CountAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity> specification)
    {
        return await ApplySpecification(specification).AnyAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        if (specification == null) return _dbSet.AsQueryable();
        return SpecificationEvaluator<TEntity>.GetQuery(_dbSet.AsQueryable(), specification);
    }
"@
                    $content = $content.Insert($classEndPos, $implText)
                }
            }
        }
        
        # Write back modified content
        Set-Content -Path $filePath -Value $content
    }
}

# Update the specific repositories causing issues
Get-ChildItem -Path $repoDir -Filter "*.cs" -Recurse | ForEach-Object {
    Fix-RepositoryImplementations $_.FullName
}

Write-Host "Repository implementations fixed"

# Fix Identity Context
$identityContextPath = "Infrastructure\Persistence\Identity\IdentityContext.cs" 
if (Test-Path $identityContextPath) {
    $content = Get-Content $identityContextPath -Raw
    
    # Fix namespace references
    $content = $content -replace "using Domain\.Entities", "using Core.Domain.Entities"
    
    # Fix UserOTP nullability
    $content = $content -replace "DbSet<UserOTP\?>", "DbSet<UserOTP>"
    
    Set-Content -Path $identityContextPath -Value $content
    Write-Host "Fixed Identity Context"
}

# Fix StoreContext references
$storeContextPath = "Infrastructure\Persistence\Data\StoreContext.cs"
if (Test-Path $storeContextPath) {
    $content = Get-Content $storeContextPath -Raw
    
    # Fix OrderEntity reference
    $content = $content -replace "OrderEntity", "Order"
    
    # Remove duplicate using directives
    $content = $content -replace "using Core\.Domain\.Entities\.HealthcareEntities;(\s+using Core\.Domain\.Entities\.HealthcareEntities;)+", "using Core.Domain.Entities.HealthcareEntities;"
    
    Set-Content -Path $storeContextPath -Value $content
    Write-Host "Fixed Store Context"
}

# Fix MedicationRepository
$medicationRepoPath = "Infrastructure\Persistence\Repositories\MedicationRepository.cs"
if (Test-Path $medicationRepoPath) {
    $content = Get-Content $medicationRepoPath -Raw
    
    # Fix namespace references
    $content = $content -replace "using Core\.Services", "using Core.Domain.Contracts"
    
    Set-Content -Path $medicationRepoPath -Value $content
    Write-Host "Fixed Medication Repository"
}

Write-Host "All fixes applied successfully" 