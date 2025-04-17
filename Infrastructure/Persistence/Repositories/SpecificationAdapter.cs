using Core.Domain.Contracts;
using Core.Common.Specifications;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Adapter class to convert ISpecification to Specifications
    /// This class bridges the Core.Common.Specifications.ISpecification<T> interface
    /// with the Core.Domain.Contracts.Specifications<T> implementation.
    /// Used to prevent circular dependencies between layers while maintaining compatibility.
    /// </summary>
    public class SpecificationAdapter<T> : Core.Domain.Contracts.Specifications<T> where T : class
    {
        public SpecificationAdapter(Core.Common.Specifications.ISpecification<T> specification)
            : base(specification.Criteria ?? (x => true))
        {
            // Copy properties from ISpecification to Specifications
            // Handle includes
            foreach (var include in specification.Includes)
            {
                AddInclude(include);
            }
            
            // Handle ordering
            if (specification.OrderBy != null)
            {
                setOrderBy(specification.OrderBy);
            }
            
            if (specification.OrderByDescending != null)
            {
                setOrderByDescending(specification.OrderByDescending);
            }
            
            // Handle paging
            if (specification.IsPagingEnabled)
            {
                ApplyPagination(specification.Skip, specification.Take);
            }
        }
    }
} 