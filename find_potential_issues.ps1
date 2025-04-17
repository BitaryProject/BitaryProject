Write-Host "Finding potential issues after updating to BaseEntity<TKey>..."

Write-Host "Checking for repository interfaces that might need updating..."
Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Core\Domain\Contracts -Filter "I*Repository.cs" -Recurse | 
    ForEach-Object {
        $content = Get-Content $_.FullName
        if ($content -notmatch "IGenericRepository<\w+,\s*\w+>" -and $content -match "IGenericRepository<") {
            Write-Host "Potential issue in: $($_.FullName)"
        }
    }

Write-Host "Checking for repository implementations that might need updating..."
Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Infrastructure\Persistence\Repositories -Filter "*.cs" -Recurse | 
    ForEach-Object {
        $content = Get-Content $_.FullName
        if ($content -notmatch "GenericRepository<\w+,\s*\w+>" -and $content -match "GenericRepository<") {
            Write-Host "Potential issue in: $($_.FullName)"
        }
    }

Write-Host "Checking for entities that might still use BaseEntity without a type parameter..."
Get-ChildItem -Path D:\DotNetRoute\BitaryProject\Core\Domain\Entities -Filter "*.cs" -Recurse | 
    ForEach-Object {
        $content = Get-Content $_.FullName
        if ($content -match ":\s*BaseEntity\b" -and $content -notmatch "BaseEntity<") {
            Write-Host "Potential issue in: $($_.FullName)"
        }
    }

Write-Host "Check complete!" 