using Domain.Entities.PetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
//using Shared.MedicalRecordModels;

namespace Shared.PetModels
{
    public record PetProfileDTO
    {
        public int Id { get; init; }
        public string PetName { get; init; }
        public DateTime BirthDate { get; init; }
        public PetGender Gender { get; init; }
        public PetType type { get; init; }
        public string Color { get; init; }
        public string Avatar { get; init; }
        public string UserId { get; init; }
       // public IEnumerable<MedicalRecordDTO> MedicalRecords { get; init; } = new List<MedicalRecordDTO>();
    }
    
    // New DTO specifically for update operations - doesn't require ID or UserID in the request
    public record UpdatePetRequestDTO
    {
        [Required]
        [StringLength(100)]
        public string PetName { get; init; }
        
        [Required]
        public DateTime BirthDate { get; init; }
        
        [Required]
        public PetGender Gender { get; init; }
        
        [Required]
        public PetType type { get; init; }
        
        [StringLength(50)]
        public string Color { get; init; }
        
        [StringLength(255)]
        public string Avatar { get; init; }
    }
}
