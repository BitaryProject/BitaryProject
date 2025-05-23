﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.MedicalRecordEntites;
namespace Domain.Entities.PetEntities
{
    public enum PetGender : byte
    {
        male = 1,
        female = 2,
        m = 1,
        f = 2
    }
    public enum PetType : byte
    {
        dog = 1,
        cat = 2,
        d = 1,
        c = 2
    }
    public class Pet : BaseEntity<int>
    {
        public Pet() { }
        public Pet(string petName, DateTime birthDate, PetGender gender, string color, string avatar, PetType type, string userId)
        {
            PetName = petName;
            BirthDate = birthDate;
            Gender = gender;
            Color = color;
            Avatar = avatar;
            PetType = type;
            UserId = userId;
        }

        public string PetName { get; set; }
        public DateTime BirthDate { get; set; }
        public PetGender Gender { get; set; }
        public PetType PetType { get; set; }
        public string Color { get; set; }
        public string Avatar { get; set; }
        
        // Reference to the user that owns this pet
        public string UserId { get; set; }

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
