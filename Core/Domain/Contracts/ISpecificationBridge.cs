using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Common.Specifications;

namespace Core.Domain.Contracts
{
    /// <summary>
    /// Interface bridge to enable conversion between ISpecification types
    /// This resolves circular dependencies between assemblies
    /// </summary>
    public interface ISpecification<T> 
    {
        Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        Expression<Func<T, object>>? GroupBy { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }

    /// <summary>
    /// Extension methods to convert between specification types
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Converts a Common.Specifications.ISpecification to a Domain.Contracts.ISpecification
        /// </summary>
        public static Core.Domain.Contracts.ISpecification<T> ToContractSpec<T>(
            this Core.Common.Specifications.ISpecification<T> spec)
        {
            return new SpecificationAdapter<T>(spec);
        }
        
        /// <summary>
        /// Converts a Domain.Contracts.ISpecification to a Common.Specifications.ISpecification
        /// </summary>
        public static Core.Common.Specifications.ISpecification<T> ToCommonSpec<T>(
            this Core.Domain.Contracts.ISpecification<T> spec)
        {
            // If it's already a SpecificationAdapter, just return the inner specification
            if (spec is SpecificationAdapter<T> adapter)
                return adapter.InnerSpecification;
                
            // Otherwise, create a new adapter
            return new CommonSpecificationAdapter<T>(spec);
        }
    }

    /// <summary>
    /// Adapter to convert from Common.Specifications.ISpecification to Domain.Contracts.ISpecification
    /// </summary>
    internal class SpecificationAdapter<T> : ISpecification<T>
    {
        private readonly Core.Common.Specifications.ISpecification<T> _specification;

        public SpecificationAdapter(Core.Common.Specifications.ISpecification<T> specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        // Allow access to the inner specification for efficient conversion back
        public Core.Common.Specifications.ISpecification<T> InnerSpecification => _specification;

        public Expression<Func<T, bool>>? Criteria => _specification.Criteria;
        public List<Expression<Func<T, object>>> Includes => _specification.Includes;
        public List<string> IncludeStrings => _specification.IncludeStrings;
        public Expression<Func<T, object>>? OrderBy => _specification.OrderBy;
        public Expression<Func<T, object>>? OrderByDescending => _specification.OrderByDescending;
        public Expression<Func<T, object>>? GroupBy => _specification.GroupBy;
        public int Take => _specification.Take;
        public int Skip => _specification.Skip;
        public bool IsPagingEnabled => _specification.IsPagingEnabled;
    }
    
    /// <summary>
    /// Adapter to convert from Domain.Contracts.ISpecification to Common.Specifications.ISpecification
    /// </summary>
    internal class CommonSpecificationAdapter<T> : Core.Common.Specifications.ISpecification<T>
    {
        private readonly Core.Domain.Contracts.ISpecification<T> _specification;
        
        public CommonSpecificationAdapter(Core.Domain.Contracts.ISpecification<T> specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }
        
        public Expression<Func<T, bool>>? Criteria => _specification.Criteria;
        public List<Expression<Func<T, object>>> Includes => _specification.Includes;
        public List<string> IncludeStrings => _specification.IncludeStrings;
        public Expression<Func<T, object>>? OrderBy => _specification.OrderBy;
        public Expression<Func<T, object>>? OrderByDescending => _specification.OrderByDescending;
        public Expression<Func<T, object>>? GroupBy => _specification.GroupBy;
        public int Take => _specification.Take;
        public int Skip => _specification.Skip;
        public bool IsPagingEnabled => _specification.IsPagingEnabled;
        
        public void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            // Not implemented since Domain.Contracts.ISpecification doesn't have these methods
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
        
        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
        
        public void AddInclude(string includeString)
        {
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
        
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
        
        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) 
        {
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
        
        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
        
        public void ApplyPaging(int skip, int take)
        {
            throw new NotSupportedException("Cannot modify a bridge specification");
        }
    }
} 