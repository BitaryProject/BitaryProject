using Domain.Entities.HealthcareEntities;
using Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class PetOwnerSpecification : BaseSpecification<PetOwner>
    {
        public PetOwnerSpecification(Guid id) 
            : base(p => p.Id == id)
        {
            AddInclude(p => p.Pets);
        }

        public PetOwnerSpecification(string userId)
            : base(p => p.UserId == userId)
        {
            AddInclude(p => p.Pets);
        }

        public PetOwnerSpecification(string email, bool exactMatch = false)
            : base(p => exactMatch ? p.Email == email : p.Email.ToLower().Contains(email.ToLower()))
        {
            AddInclude(p => p.Pets);
        }

        public PetOwnerSpecification(int pageIndex, int pageSize)
            : base(null)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(p => p.FullName);
            AddInclude(p => p.Pets);
        }

        public PetOwnerSpecification(Expression<Func<PetOwner, bool>> criteria)
            : base(criteria)
        {
            AddInclude(p => p.Pets);
        }

        public PetOwnerSpecification()
            : base(null)
        {
            AddOrderBy(p => p.FullName);
            AddInclude(p => p.Pets);
        }
    }
} 