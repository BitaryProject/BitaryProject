Write-Host "Updating repository implementations to use GenericRepository<T, Guid>..."

# Find all files containing "GenericRepository<" but not containing "GenericRepository<T, "
$files = Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Infrastructure\Persistence\Repositories -Filter "*.cs" -Recurse |
         Select-String -Pattern ":\s*GenericRepository<\w+>" -SimpleMatch |
         Where-Object { $_.Line -notmatch "GenericRepository<\w+,\s*\w+>" } |
         Select-Object -ExpandProperty Path -Unique

foreach ($file in $files) {
    Write-Host "Updating file: $file"
    
    # Read the file content
    $content = Get-Content -Path $file
    
    # Replace GenericRepository<T> with GenericRepository<T, Guid>
    $updatedContent = $content -replace "GenericRepository<(\w+)>", "GenericRepository<`$1, Guid>"
    
    # Write the updated content back to the file
    Set-Content -Path $file -Value $updatedContent
}

Write-Host "Update complete!" 