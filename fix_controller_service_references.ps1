$controllers = Get-ChildItem -Path "Infrastructure/Presentation/Controllers" -Filter "*.cs"

foreach ($controller in $controllers) {
    $content = Get-Content -Path $controller.FullName -Raw
    
    # Fix common namespace issues
    $content = $content -replace "using API.Extensions;", "using Infrastructure.Presentation.Extensions;"
    $content = $content -replace "using Services.Abstractions;", "using Core.Services.Abstractions;"
    
    # Add missing namespace at the top of the file
    if (-not ($content -match "using Core.Services.Abstractions;")) {
        $content = $content -replace "using Microsoft\.AspNetCore\.Mvc;", "using Microsoft.AspNetCore.Mvc;`nusing Core.Services.Abstractions;"
    }
    
    # Fix nullable reference warnings
    $content = $content -replace "string specialty = null", "string? specialty = null"
    $content = $content -replace "string city = null", "string? city = null"
    
    # Write the content back to the file
    Set-Content -Path $controller.FullName -Value $content
    
    Write-Host "Fixed: $($controller.Name)"
}

Write-Host "All controllers fixed!" 