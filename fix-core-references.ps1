Write-Host "Fixing Core.Core references throughout the codebase..."

# Get all .cs files in the project
$files = Get-ChildItem -Path "." -Filter "*.cs" -Recurse -Exclude "bin\*", "obj\*"

foreach ($file in $files) {
    Write-Host "Processing file: $($file.FullName)"
    
    # Read the content of the file
    $content = Get-Content -Path $file.FullName -Raw
    
    # Fix Core references
    $contentChanged = $false
    $newContent = $content
    
    # Check if we have any Core.Core references
    if ($content -match "Core\.Core") {
        $newContent = $content -replace "using Core\.Core\.", "using Core."
        $newContent = $newContent -replace "Core\.Core\.", "Core."
        $contentChanged = $true
    }
    
    # Fix Core namespace references with redundant Common.Specifications.Common.Specifications
    if ($content -match "Core\.Common\.Specifications\.Common\.Specifications") {
        $newContent = $newContent -replace "Core\.Common\.Specifications\.Common\.Specifications\.Core\.Common\.Specifications\.", "Core.Common.Specifications."
        $newContent = $newContent -replace "Core\.Common\.Specifications\.Common\.Specifications\.", "Core.Common.Specifications."
        $contentChanged = $true
    }
    
    # Fix ISpecification interface references
    if ($content -match "Core\.Common\.Specifications\.Core\.") {
        $newContent = $newContent -replace "Core\.Common\.Specifications\.Core\.", "Core."
        $contentChanged = $true
    }
    
    # Only write if we've made changes
    if ($contentChanged) {
        # Write the modified content back to the file
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        Write-Host "Updated: $($file.FullName)"
    }
}

Write-Host "Done fixing Core.Core references!" 