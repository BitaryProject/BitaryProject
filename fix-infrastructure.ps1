Write-Host "Starting comprehensive infrastructure project fix..."

# Fix global using directives
Write-Host "1. Fixing global using directives in configuration files..."

$configFiles = Get-ChildItem -Path "Infrastructure\Persistence\Data\Configurations" -Filter "*.cs"
foreach ($file in $configFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Update global using directives
    $content = $content -replace "global using (.*?)Domain\.Entities", "global using `$1Core.Domain.Entities"
    
    # Fix namespace
    $content = $content -replace "namespace Persistence\.Data\.Configurations", "namespace Infrastructure.Persistence.Data.Configurations"
    
    # Ensure Core prefix in using directives
    $content = $content -replace "using Domain\.Entities", "using Core.Domain.Entities"
    
    # Ensure Microsoft.EntityFrameworkCore is included
    if (-not ($content -match "using Microsoft\.EntityFrameworkCore;")) {
        $content = $content -replace "using Microsoft\.EntityFrameworkCore\.Metadata\.Builders;", "using Microsoft.EntityFrameworkCore;`nusing Microsoft.EntityFrameworkCore.Metadata.Builders;"
    }
    
    # Write the updated content back to the file
    Set-Content -Path $file.FullName -Value $content
}

# Fix StoreContext references
Write-Host "2. Fixing StoreContext references..."

$files = Get-ChildItem -Path "Infrastructure\Persistence" -Recurse -Include "*.cs" | 
         Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" }

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Fix namespace in file
    $content = $content -replace "namespace Persistence\.", "namespace Infrastructure.Persistence."
    
    # Fix using directives for Persistence namespace
    $content = $content -replace "using Persistence\.", "using Infrastructure.Persistence."
    
    # Fix any references to Domain without Core prefix
    $content = $content -replace "using Domain\.", "using Core.Domain."
    
    # Fix Services specifications
    $content = $content -replace "using Services\.Specifications", "using Core.Services.Specifications"
    
    # Write the updated content back to the file
    Set-Content -Path $file.FullName -Value $content
}

# Fix ISpecification ambiguity
Write-Host "3. Fixing ISpecification ambiguity in repository files..."

$repositoryFiles = Get-ChildItem -Path "Infrastructure\Persistence\Repositories" -Recurse -Include "*.cs" | 
                  Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" }

foreach ($file in $repositoryFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Replace ambiguous ISpecification usage with fully qualified names
    $content = $content -replace "ISpecification<([^>]+)>", "Core.Common.Specifications.ISpecification<`$1>"
    
    # Write the updated content back to the file
    Set-Content -Path $file.FullName -Value $content
}

Write-Host "4. Fixing repository base classes and interfaces..."

# Add message about manual changes needed for repository implementations
Write-Host @"
Infrastructure fixes applied.

Note: Some repository implementation issues will need to be fixed manually:
1. Review repository implementations that don't match their interfaces
2. Check for methods with incorrect return types
3. Add the 'new' keyword to methods that intentionally hide base class methods

Build the project again to see remaining issues.
"@ 