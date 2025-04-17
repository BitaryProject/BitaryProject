$controllers = Get-ChildItem -Path "Infrastructure/Presentation" -Filter "*Controller.cs" 

foreach ($controller in $controllers) {
    $content = Get-Content -Path $controller.FullName -Raw
    
    # Fix namespace references
    $content = $content -replace "using Services.Abstractions;", "using Core.Services.Abstractions;"
    $content = $content -replace "using Services;", "using Core.Services;"
    
    # Add missing namespace at the top of the file
    if (-not ($content -match "using Core.Services.Abstractions;")) {
        $content = $content -replace "using Microsoft\.AspNetCore\.Mvc;", "using Microsoft.AspNetCore.Mvc;`nusing Core.Services.Abstractions;"
    }
    
    # Write the content back to the file
    Set-Content -Path $controller.FullName -Value $content
    
    Write-Host "Fixed: $($controller.Name)"
}

Write-Host "All root controllers fixed!" 