# Fix StoreContext.cs to include Healthcare entities
$repoPath = "Infrastructure\Persistence\Data\StoreContext.cs"
if (Test-Path $repoPath) {
    # Read the existing file
    $content = Get-Content $repoPath -Raw
    
    # Add missing using statements
    $usingStatements = @"
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Domain.Entities;
using Core.Domain.Entities.HealthcareEntities;
using Core.Domain.Entities.IdentityEntities;
using Core.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;

"@
    
    # Replace the existing using statements
    $content = $content -replace "(using.*\r\n)+", $usingStatements
    
    # Fix DbSet properties if needed
    if ($content -notmatch "public DbSet<PetOwner> PetOwners { get; set; }") {
        $insertPoint = $content.LastIndexOf("protected override void OnModelCreating")
        
        $dbSetProperties = @"
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
    public DbSet<CustomerBasket> CustomerBaskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    
    // Healthcare entities
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<PetOwner> PetOwners { get; set; }
    public DbSet<PetProfile> PetProfiles { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<MedicalNote> MedicalNotes { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<DoctorRating> DoctorRatings { get; set; }
    public DbSet<ClinicRating> ClinicRatings { get; set; }
    
    "
        
        $content = $content.Insert($insertPoint, $dbSetProperties)
    }
    
    Set-Content -Path $repoPath -Value $content
    Write-Host "Fixed StoreContext.cs"
} 