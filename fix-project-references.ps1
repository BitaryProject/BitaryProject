# Fix Project References Script
# This script checks and fixes project references between Persistence, Core, and Domain projects

# First, let's check the Persistence.csproj file and add necessary references
$persistenceProjPath = "Infrastructure\Persistence\Persistence.csproj"
if (Test-Path $persistenceProjPath) {
    $persistenceProj = Get-Content $persistenceProjPath -Raw
    $modified = $false
    
    # Check if Core.Common reference is missing and add it if needed
    if ($persistenceProj -notmatch '<ProjectReference Include="..\\..\\Core\\Common\\Common.csproj" />') {
        Write-Host "Adding Core.Common reference to Persistence project"
        $persistenceProj = $persistenceProj -replace '(<ItemGroup>[\s\S]*?)</ItemGroup>', "`$1`n    <ProjectReference Include=`"..\\..\\Core\\Common\\Common.csproj`" />`n  </ItemGroup>"
        $modified = $true
    }
    
    # Check if Core.Domain reference is missing and add it if needed
    if ($persistenceProj -notmatch '<ProjectReference Include="..\\..\\Core\\Domain\\Domain.csproj" />') {
        Write-Host "Adding Core.Domain reference to Persistence project"
        $persistenceProj = $persistenceProj -replace '(<ItemGroup>[\s\S]*?)</ItemGroup>', "`$1`n    <ProjectReference Include=`"..\\..\\Core\\Domain\\Domain.csproj`" />`n  </ItemGroup>"
        $modified = $true
    }
    
    # Add missing namespaces in StoreContext.cs
    $storeContextPath = "Infrastructure\Persistence\Data\StoreContext.cs"
    if (Test-Path $storeContextPath) {
        $storeContext = Get-Content $storeContextPath -Raw
        
        # Add missing using directives
        if ($storeContext -notmatch "using System.Linq.Expressions;") {
            $storeContext = "using System.Linq.Expressions;`r`n$storeContext"
        }
        
        # Add missing entity imports
        if ($storeContext -notmatch "using Core.Domain.Entities.IdentityEntities;") {
            $storeContext = "using Core.Domain.Entities.IdentityEntities;`r`n$storeContext"
        }
        
        # Fix double Core references (Core.Core) that might have been introduced
        $storeContext = $storeContext -replace "Core\.Core\.", "Core."
        
        Set-Content -Path $storeContextPath -Value $storeContext
        Write-Host "Fixed StoreContext.cs imports"
    }
    
    # Add missing using statements to ClinicRatingRepository.cs
    $clinicRatingRepoPath = "Infrastructure\Persistence\Repositories\HealthcareRepositories\ClinicRatingRepository.cs"
    if (Test-Path $clinicRatingRepoPath) {
        $clinicRatingRepo = Get-Content $clinicRatingRepoPath -Raw
        
        # Add missing using directives
        $newContent = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.HealthcareRepositories
{
"@
        
        $clinicRatingRepo = $clinicRatingRepo -replace "using.*\r\n(using.*\r\n)*namespace Infrastructure.Persistence.Repositories.HealthcareRepositories(\r\n)?{", $newContent
        
        Set-Content -Path $clinicRatingRepoPath -Value $clinicRatingRepo
        Write-Host "Fixed ClinicRatingRepository.cs imports"
    }
    
    # Fix IdentityContext.cs
    $identityContextPath = "Infrastructure\Persistence\Identity\IdentityContext.cs"
    if (Test-Path $identityContextPath) {
        $identityContext = Get-Content $identityContextPath -Raw
        
        # Add missing using directives
        if ($identityContext -notmatch "using Core.Domain.Entities.IdentityEntities;") {
            $identityContext = "using Core.Domain.Entities.IdentityEntities;`r`n$identityContext"
        }
        
        # Fix double Core references (Core.Core) that might have been introduced
        $identityContext = $identityContext -replace "Core\.Core\.", "Core."
        
        Set-Content -Path $identityContextPath -Value $identityContext
        Write-Host "Fixed IdentityContext.cs imports"
    }
    
    # Fix MedicationRepository.cs
    $medicationRepoPath = "Infrastructure\Persistence\Repositories\MedicationRepository.cs"
    if (Test-Path $medicationRepoPath) {
        $medicationRepo = Get-Content $medicationRepoPath -Raw
        
        # Fix invalid using statements
        $medicationRepo = $medicationRepo -replace "using Specifications<T>;", ""
        $medicationRepo = $medicationRepo -replace "using Specifications;", ""
        
        # Add proper namespace imports
        if ($medicationRepo -notmatch "using System.Linq.Expressions;") {
            $medicationRepo = "using System.Linq.Expressions;`r`n$medicationRepo"
        }
        
        Set-Content -Path $medicationRepoPath -Value $medicationRepo
        Write-Host "Fixed MedicationRepository.cs imports"
    }
    
    # Save the modified Persistence.csproj if changes were made
    if ($modified) {
        Set-Content -Path $persistenceProjPath -Value $persistenceProj
        Write-Host "Updated Persistence.csproj with required project references"
    }
    
    # Fix SpecificationEvaluator.cs to handle namespaces correctly
    $specEvalPath = "Infrastructure\Persistence\Repositories\SpecificationEvaluator.cs"
    if (Test-Path $specEvalPath) {
        $specEvalContent = Get-Content $specEvalPath -Raw
        
        # Fix ISpecification namespaces to be fully qualified
        $specEvalContent = $specEvalContent -replace "Core\.Common\.Specifications\.Core\.Common\.Specifications\.", "Core.Common.Specifications."
        
        # Ensure correct using statements
        if ($specEvalContent -notmatch "using System.Linq;") {
            $specEvalContent = "using System.Linq;`r`n$specEvalContent"
        }
        
        Set-Content -Path $specEvalPath -Value $specEvalContent
        Write-Host "Fixed SpecificationEvaluator.cs"
    }
    
    # Fix OrderConfigurations.cs
    $orderConfigPath = "Infrastructure\Persistence\Data\Configurations\OrderConfigurations.cs"
    if (Test-Path $orderConfigPath) {
        $orderConfigContent = Get-Content $orderConfigPath -Raw
        
        # Fix Core references
        $orderConfigContent = $orderConfigContent -replace "Core\.Core\.", "Core."
        
        # Ensure correct using statements for entities
        if ($orderConfigContent -notmatch "using Core.Domain.Entities.OrderEntities;") {
            $orderConfigContent = "using Core.Domain.Entities.OrderEntities;`r`n$orderConfigContent"
        }
        
        Set-Content -Path $orderConfigPath -Value $orderConfigContent
        Write-Host "Fixed OrderConfigurations.cs"
    }
    
    Write-Host "Project reference fixes complete."
}
else {
    Write-Host "Persistence.csproj not found at expected location: $persistenceProjPath"
} 