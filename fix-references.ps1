# Scan for files with references to BaseEntity
$entityFiles = Get-ChildItem -Path "Core/Domain/Entities" -Recurse -Filter "*.cs" -Exclude "BaseEntity.cs"

foreach ($file in $entityFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $updated = $content
    
    # Fix namespace references
    $updated = $updated -replace "using Core\.Domain\.Entities\.Base;", "using Core.Domain.Entities;"
    $updated = $updated -replace "using Domain\.Entities\.Base;", "using Core.Domain.Entities;"
    
    # Fix BaseEntity references
    $updated = $updated -replace "BaseEntity<>", "BaseEntity<Guid>"
    
    # Fix namespace for Domain.Entities to Core.Domain.Entities
    $updated = $updated -replace "namespace Domain\.Entities", "namespace Core.Domain.Entities"
    $updated = $updated -replace "using Domain\.Entities", "using Core.Domain.Entities"
    
    # Only write if content has changed
    if ($content -ne $updated) {
        Write-Host "Fixing entity references in $($file.Name)"
        Set-Content -Path $file.FullName -Value $updated
    }
}

# Scan for repository interface files
$repositoryFiles = Get-ChildItem -Path "Core/Domain/Contracts" -Filter "I*Repository.cs"

foreach ($file in $repositoryFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $updated = $content
    
    # Fix IGenericRepository<> without closing >
    $updated = $updated -replace "IGenericRepository<([^>]+)<", "IGenericRepository<`$1, "
    $updated = $updated -replace "IGenericRepository<([^>]+)>", "IGenericRepository<`$1, Guid>"
    
    # Only write if content has changed
    if ($content -ne $updated) {
        Write-Host "Fixing repository references in $($file.Name)"
        Set-Content -Path $file.FullName -Value $updated
    }
}

Write-Host "Reference fixes completed!" 