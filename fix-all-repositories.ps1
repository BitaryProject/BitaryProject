# Fix all repository interfaces
$repositoryFiles = Get-ChildItem -Path "Core/Domain/Contracts" -Filter "I*.cs"

foreach ($file in $repositoryFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $origContent = $content
    
    # Fix namespace
    $content = $content -replace "namespace Domain\.Contracts", "namespace Core.Domain.Contracts"
    
    # Fix IGenericRepository with 3 type parameters
    $content = $content -replace "IGenericRepository<([^,]+), ([^,]+), ([^>]+)>", "IGenericRepository<`$1, `$2>"
    
    # Fix IGenericRepository with comma issues
    $content = $content -replace "IGenericRepository<([^,]+),\s*>", "IGenericRepository<`$1, Guid>"
    
    # Write content if changed
    if ($origContent -ne $content) {
        Write-Host "Fixing $($file.Name)"
        Set-Content -Path $file.FullName -Value $content
    }
}

Write-Host "Repository fixes completed!" 