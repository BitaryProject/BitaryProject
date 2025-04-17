using System;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;

namespace Core.Domain.Entities.HealthcareEntities
{
    public class ClinicRating : BaseEntity<Guid>
    {
        public Guid ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
        
        public Guid PetOwnerId { get; set; }
        public virtual PetOwner PetOwner { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(500)]
        public string Comment { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
} 
