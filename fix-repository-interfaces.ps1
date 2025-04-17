# Get all repository interfaces
$repositoryFiles = Get-ChildItem -Path "Core/Domain/Contracts" -Filter "I*Repository.cs" -Exclude "IGenericRepository.cs"

foreach ($file in $repositoryFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $updated = $content
    
    # Fix namespace references
    $updated = $updated -replace "using Domain\.", "using Core.Domain."
    $updated = $updated -replace "namespace Domain\.", "namespace Core.Domain."
    
    # Fix IGenericRepository references with three parameters
    $updated = $updated -replace "IGenericRepository<([^>]+), ([^>]+), Guid>", "IGenericRepository<`$1, `$2>"
    
    # Fix IGenericRepository references with two parameters resulting from our previous script
    $updated = $updated -replace "IGenericRepository<([^>]+), Guid>", "IGenericRepository<`$1, Guid>"
    
    # Only write if content has changed
    if ($content -ne $updated) {
        Write-Host "Fixing repository interface in $($file.Name)"
        Set-Content -Path $file.FullName -Value $updated
    }
}

Write-Host "Repository interface fixes completed!" 