Write-Host "Fixing configuration files in Infrastructure/Persistence/Data/Configurations..."

$configDir = "Infrastructure/Persistence/Data/Configurations"
$files = Get-ChildItem -Path $configDir -Filter "*.cs" -Recurse

foreach ($file in $files) {
    Write-Host "Processing file: $($file.FullName)"
    
    # Read the content of the file
    $content = Get-Content -Path $file.FullName -Raw
    
    # Fix global using directives
    if ($content -match "global using") {
        $content = $content -replace "global using", "using"
    }
    
    # Fix Core references
    $content = $content -replace "using Core\.Core\.", "using Core."
    $content = $content -replace "Core\.Core\.", "Core."
    
    # Fix Core namespace references
    $content = $content -replace "using Core\.Domain", "using Core.Domain"
    $content = $content -replace "using Core\.Common", "using Core.Common"
    
    # Replace any remaining problematic patterns
    $content = $content -replace "Core\.Core", "Core"
    
    # Write the modified content back to the file
    Set-Content -Path $file.FullName -Value $content -NoNewline
}

Write-Host "Done fixing configuration files!" 