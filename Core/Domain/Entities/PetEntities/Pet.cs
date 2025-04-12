/*using System;
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
        public class Pet : BaseEntity<Guid>
        {
        public Pet() { }
        public Pet(string userId, string petName, DateTime birthDate, PetGender gender, string color, string avatar , PetType type)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            PetName = petName;
            BirthDate = birthDate;
            Gender = gender;
            Color = color;
            Avatar = avatar;
            PetType = type;
           
        }


            public string PetName { get; set; }

            public DateTime BirthDate { get; set; }

            public PetGender Gender { get; set; }
            public PetType PetType { get; set; }
            public string Color { get; set; }
            public string Avatar { get; set; }
            public string UserId { get; set; }

            public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        }
    }
*/