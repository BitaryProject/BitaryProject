Write-Host "Updating repository interfaces to use IGenericRepository<T, Guid>..."

# Find all files containing "IGenericRepository<" but not containing "IGenericRepository<T, "
$files = Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Core\Domain\Contracts -Filter "*.cs" -Recurse |
         Select-String -Pattern "IGenericRepository<\w+>" -SimpleMatch |
         Where-Object { $_.Line -notmatch "IGenericRepository<\w+,\s*\w+>" } |
         Select-Object -ExpandProperty Path -Unique

foreach ($file in $files) {
    Write-Host "Updating file: $file"
    
    # Read the file content
    $content = Get-Content -Path $file
    
    # Replace IGenericRepository<T> with IGenericRepository<T, Guid>
    $updatedContent = $content -replace "IGenericRepository<(\w+)>", "IGenericRepository<`$1, Guid>"
    
    # Write the updated content back to the file
    Set-Content -Path $file -Value $updatedContent
}

Write-Host "Update complete!" 