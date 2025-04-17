using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System;
using System.Linq.Expressions;

namespace Core.Services.Specifications
{
    public class PetOwnerSpecification : BaseSpecification<PetOwner>
    {
        public PetOwnerSpecification(Guid id) 
            : base(p => p.Id == id)
        {
            AddIncludes();
        }

        public PetOwnerSpecification(string userId)
            : base(p => p.UserId == userId)
        {
            AddIncludes();
        }

        public PetOwnerSpecification(string email, bool exactMatch = false)
            : base(p => exactMatch ? p.Email == email : p.Email.ToLower().Contains(email.ToLower()))
        {
            AddIncludes();
        }

        public PetOwnerSpecification(int pageIndex, int pageSize)
            : base(null)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
            AddOrderBy(p => p.FullName);
            AddIncludes();
        }

        public PetOwnerSpecification(Expression<Func<PetOwner, bool>> criteria)
            : base(criteria)
        {
            AddIncludes();
        }

        public PetOwnerSpecification()
            : base(null)
        {
            AddOrderBy(p => p.FullName);
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(p => p.Pets);
        }
    }
} 
