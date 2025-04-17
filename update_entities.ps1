Write-Host "Updating entity files to use BaseEntity<Guid>..."

# Find all files containing "BaseEntity" but not containing "BaseEntity<"
$files = Get-ChildItem -Path . -Filter "*.cs" -Recurse | 
         Select-String -Pattern ":\s*BaseEntity\b" |
         Where-Object { $_.Line -notmatch "BaseEntity<" } |
         Select-Object -ExpandProperty Path -Unique

foreach ($file in $files) {
    Write-Host "Updating file: $file"
    
    # Read the file content
    $content = Get-Content -Path $file
    
    # Replace : BaseEntity with : BaseEntity<Guid>
    $updatedContent = $content -replace ":\s*BaseEntity\b", ": BaseEntity<Guid>"
    
    # Write the updated content back to the file
    Set-Content -Path $file -Value $updatedContent
}

Write-Host "Update complete!" 