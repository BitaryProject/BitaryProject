using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class PetSpecification : BaseSpecification<PetProfile>
    {
        public PetSpecification(Guid id) 
            : base(p => p.Id == id)
        {
            AddInclude(p => p.Owner);
            AddInclude(p => p.MedicalRecords);
            AddInclude(p => p.Appointments);
        }

        public PetSpecification(Guid ownerId, int pageIndex, int pageSize)
            : base(p => p.OwnerId == ownerId)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(p => p.PetName);
            AddInclude(p => p.Owner);
        }

        public PetSpecification(string name, int pageIndex, int pageSize)
            : base(p => string.IsNullOrEmpty(name) || p.PetName.ToLower().Contains(name.ToLower()))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(p => p.PetName);
            AddInclude(p => p.Owner);
        }

        public PetSpecification(string species, string breed, int pageIndex, int pageSize)
            : base(p => 
                (string.IsNullOrEmpty(species) || p.Species.ToLower().Contains(species.ToLower())) &&
                (string.IsNullOrEmpty(breed) || p.Breed.ToLower().Contains(breed.ToLower())))
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(p => p.PetName);
            AddInclude(p => p.Owner);
        }

        public PetSpecification(Expression<Func<PetProfile, bool>> criteria)
            : base(criteria)
        {
            AddInclude(p => p.Owner);
        }

        public PetSpecification()
            : base(null)
        {
            AddOrderBy(p => p.PetName);
            AddInclude(p => p.Owner);
        }
    }
}
