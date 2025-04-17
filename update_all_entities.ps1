Write-Host "Updating all entities to use BaseEntity<Guid>..."

# Find all files that inherit from BaseEntity without a type parameter
$entityFiles = Get-ChildItem -Path D:\DotNetRoute\BitaryProject -Filter "*.cs" -Recurse |
               Select-String -Pattern ":\s*BaseEntity\b" -SimpleMatch |
               Where-Object { $_.Line -notmatch "BaseEntity<" } |
               Select-Object -ExpandProperty Path -Unique

foreach ($file in $entityFiles) {
    Write-Host "Updating entity file: $file"
    
    $content = Get-Content -Path $file
    $updatedContent = $content -replace ":\s*BaseEntity\b", ": BaseEntity<Guid>"
    Set-Content -Path $file -Value $updatedContent
}

# Find all repository interfaces that need to be updated
$interfaceFiles = Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Core\Domain\Contracts -Filter "*.cs" -Recurse |
                  Select-String -Pattern "IGenericRepository<\w+>" -SimpleMatch |
                  Where-Object { $_.Line -notmatch "IGenericRepository<\w+,\s*\w+>" } |
                  Select-Object -ExpandProperty Path -Unique

foreach ($file in $interfaceFiles) {
    Write-Host "Updating interface file: $file"
    
    $content = Get-Content -Path $file
    $updatedContent = $content -replace "IGenericRepository<(\w+)>", "IGenericRepository<`$1, Guid>"
    Set-Content -Path $file -Value $updatedContent
}

# Find all repository implementations that need to be updated
$repositoryFiles = Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Infrastructure\Persistence\Repositories -Filter "*.cs" -Recurse |
                   Select-String -Pattern "GenericRepository<\w+>" -SimpleMatch |
                   Where-Object { $_.Line -notmatch "GenericRepository<\w+,\s*\w+>" } |
                   Select-Object -ExpandProperty Path -Unique

foreach ($file in $repositoryFiles) {
    Write-Host "Updating repository file: $file"
    
    $content = Get-Content -Path $file
    $updatedContent = $content -replace "GenericRepository<(\w+)>", "GenericRepository<`$1, Guid>"
    Set-Content -Path $file -Value $updatedContent
}

Write-Host "All updates complete!" 