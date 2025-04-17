$controllers = Get-ChildItem -Path "Infrastructure/Presentation/Controllers" -Filter "*.cs"

foreach ($controller in $controllers) {
    $content = Get-Content -Path $controller.FullName -Raw
    
    # Fix namespace issues
    $content = $content -replace "using API.Extensions;", "using Infrastructure.Presentation.Extensions;"
    $content = $content -replace "using Services.Abstractions;", "using Core.Services.Abstractions;"
    
    # Fix any other common issues
    Set-Content -Path $controller.FullName -Value $content
    
    Write-Host "Fixed: $($controller.Name)"
}

Write-Host "All controllers fixed!" 