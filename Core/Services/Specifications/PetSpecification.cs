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
       
        public PetSpecification(Guid id)
            : base(p => p.Id == id)
        {
          
            AddInclude(p => p.MedicalRecords);
        }

        public PetSpecification(string userId, int pageIndex, int pageSize, string? petName = null)
            : base(p => p.UserId == userId &&
                        (string.IsNullOrWhiteSpace(petName) || p.PetName.ToLower().Contains(petName.ToLower())))
        {
            ApplyPagination(pageIndex, pageSize);
            setOrderBy(p => p.PetName);
        }
    }
}
