$serviceInterfaces = Get-ChildItem -Path "Core/Services.Abstractions" -Filter "*.cs"

foreach ($interface in $serviceInterfaces) {
    $content = Get-Content -Path $interface.FullName -Raw
    
    # Fix namespace
    $content = $content -replace "namespace Services.Abstractions", "namespace Core.Services.Abstractions"
    
    # Write the content back to the file
    Set-Content -Path $interface.FullName -Value $content
    
    Write-Host "Fixed: $($interface.Name)"
}

Write-Host "All service interfaces fixed!" 