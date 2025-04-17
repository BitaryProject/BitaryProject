$files = Get-ChildItem -Path "Core/Domain/Entities" -Recurse -Filter "*.cs" -Exclude "BaseEntity.cs"

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName
    $updated = $content -replace "using Core\.Domain\.Entities\.Base;", "using Core.Domain.Entities;"
    $updated = $updated -replace "BaseEntity<>", "BaseEntity<Guid>"
    
    # Only write if content has changed
    if ($content -ne $updated) {
        Write-Host "Fixing namespace in $($file.Name)"
        Set-Content -Path $file.FullName -Value $updated
    }
}

Write-Host "Namespace fix completed!" 