using Domain.Entities.PetEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.PetModels
{
    public class PetDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PetName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public PetGender Gender { get; set; }

        [Required]
        public PetType PetType { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }
        
        // Reference to the user that owns this pet
        public string UserId { get; set; }
    }

    public class CreatePetDTO
    {
        [Required]
        [StringLength(100)]
        public string PetName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public PetGender Gender { get; set; }

        [Required]
        public PetType PetType { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }
    }

    public class UpdatePetDTO
    {
        [Required]
        [StringLength(100)]
        public string PetName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public PetGender Gender { get; set; }

        [Required]
        public PetType PetType { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }
    }
} 