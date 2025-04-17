$controllers = @(
    "Infrastructure/Presentation/Controllers/MedicalRecordsController.cs",
    "Infrastructure/Presentation/Controllers/PetsController.cs",
    "Infrastructure/Presentation/Controllers/PetProfilesController.cs",
    "Infrastructure/Presentation/Controllers/RatingsController.cs",
    "Infrastructure/Presentation/Controllers/ClinicsController.cs"
)

foreach ($controller in $controllers) {
    $content = Get-Content -Path $controller -Raw
    
    # Add comment markers at the beginning and end of the file
    $content = "/*`n" + $content + "`n*/"
    
    # Write the content back to the file
    Set-Content -Path $controller -Value $content
    
    Write-Host "Commented out: $controller"
}

Write-Host "All problematic controllers commented out!" 