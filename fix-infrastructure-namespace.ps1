Write-Host "Fixing namespace issues with ISpecification and Core references..."

$files = Get-ChildItem -Path "Infrastructure\Persistence\Repositories" -Recurse -Include "*.cs" | 
         Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" }

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Fix Core.Common.Specifications.Core -> Core.Common.Specifications
    $content = $content -replace "Core\.Common\.Specifications\.Core", "Core.Common.Specifications"
    
    # Fix Core.Domain.Contracts.Core -> Core.Domain.Contracts
    $content = $content -replace "Core\.Domain\.Contracts\.Core", "Core.Domain.Contracts"
    
    # Use qualified type names for ISpecification
    $content = $content -replace "ISpecification<([^>]+)>", "Core.Common.Specifications.ISpecification<`$1>"
    
    # Write the updated content back to the file
    Set-Content -Path $file.FullName -Value $content
}

Write-Host "Looking for configuration files with incorrect entity references..."

$configFiles = Get-ChildItem -Path "Infrastructure\Persistence\Data\Configurations" -Filter "*.cs"
foreach ($file in $configFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Fix global using
    $content = $content -replace "global using (.+) = Core\.Core\.", "global using `$1 = Core."
    
    # Add standard using statements
    if (-not ($content -match "using Core\.Domain\.Entities;")) {
        $content = "using Core.Domain.Entities;`nusing Core.Domain.Entities.HealthcareEntities;`nusing Core.Domain.Entities.OrderEntities;`nusing Microsoft.EntityFrameworkCore;`nusing Microsoft.EntityFrameworkCore.Metadata.Builders;`nusing System;`n" + $content
    }
    
    # Set-Content -Path $file.FullName -Value $content
    Set-Content -Path $file.FullName -Value $content
}

# Fix Identity namespace references
$identityFiles = Get-ChildItem -Path "Infrastructure\Persistence\Identity" -Filter "*.cs"
foreach ($file in $identityFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Fix Domain.Entities
    $content = $content -replace "using Domain\.Entities", "using Core.Domain.Entities"
    
    # Write the updated content back to the file
    Set-Content -Path $file.FullName -Value $content
}

Write-Host "Namespace fixes applied. Many manual repository implementation fixes will still be needed." 