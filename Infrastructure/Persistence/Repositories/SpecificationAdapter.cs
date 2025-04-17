using Core.Common.Specifications;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Adapter class to convert ISpecification to Specifications
    /// This class bridges the Core.Common.Specifications.ISpecification<T> interface
    /// with the Core.Domain.Contracts.Specifications<T> implementation.
    /// Used to prevent circular dependencies between layers while maintaining compatibility.
    /// </summary>
    public class SpecificationAdapter<T>
    {
        private readonly ISpecification<T> _specification;
        
        public SpecificationAdapter(ISpecification<T> specification)
        {
            _specification = specification;
            
            // Copy properties from ISpecification to Specifications
            Criteria = specification.Criteria;
            Includes = specification.Includes;
            IncludeStrings = specification.IncludeStrings;
            OrderBy = specification.OrderBy;
            OrderByDescending = specification.OrderByDescending;
            GroupBy = specification.GroupBy;
            Take = specification.Take;
            Skip = specification.Skip;
            IsPagingEnabled = specification.IsPagingEnabled;
        }
        
        public Expression<Func<T, bool>>? Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; private set; }
        public List<string> IncludeStrings { get; private set; }
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public Expression<Func<T, object>>? GroupBy { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }
    }
} 








