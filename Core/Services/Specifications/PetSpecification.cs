using Domain.Contracts;
using Domain.Entities.PetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class PetSpecification : Specifications<Pet>
    {
        public PetSpecification(string userId)
            : base(p => p.UserId == userId)
        {
            // Add any includes if needed
            // e.g., AddInclude(p => p.SomeRelatedEntity);
        }

        public PetSpecification(int petId)
            : base(p => p.Id == petId)
        {
            // Add any includes if needed
        }
    }
}
