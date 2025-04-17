using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Common.Specifications;
using Core.Domain.Contracts;

namespace Core.Services.Specifications
{
    /// <summary>
    /// Extension methods for converting between different specification types
    /// </summary>
    public static class SpecificationConverters
    {
        /// <summary>
        /// Converts an AppointmentSpecification to a BaseSpecification
        /// </summary>
        public static BaseSpecification<T> ToBaseSpecification<T>(this BaseSpecification<T> spec)
        {
            // We already have a BaseSpecification, so just return it
            return spec;
        }
        
        /// <summary>
        /// Converts an ISpecification from Domain.Contracts to a BaseSpecification
        /// </summary>
        public static BaseSpecification<T> ToBaseSpecification<T>(this Core.Domain.Contracts.ISpecification<T> spec)
        {
            // Create new base specification with the criteria
            var baseSpec = new BaseSpecification<T>(spec.Criteria);
            
            // Copy all includes
            foreach (var include in spec.Includes)
            {
                baseSpec.AddInclude(include);
            }
            
            // Copy all include strings
            foreach (var includeString in spec.IncludeStrings)
            {
                baseSpec.AddInclude(includeString);
            }
            
            // Copy ordering
            if (spec.OrderBy != null)
            {
                baseSpec.ApplyOrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                baseSpec.ApplyOrderByDescending(spec.OrderByDescending);
            }
            
            // Copy paging
            if (spec.IsPagingEnabled)
            {
                baseSpec.ApplyPaging(spec.Skip, spec.Take);
            }
            
            return baseSpec;
        }
        
        /// <summary>
        /// Converts a Domain.Contracts.ISpecification to a Common.Specifications.ISpecification
        /// </summary>
        public static Core.Common.Specifications.ISpecification<T> ToCommonSpec<T>(
            this Core.Domain.Contracts.ISpecification<T> spec)
        {
            return Core.Domain.Contracts.SpecificationExtensions.ToCommonSpec(spec);
        }
        
        /// <summary>
        /// Converts a Common.Specifications.ISpecification to a Domain.Contracts.ISpecification
        /// </summary>
        public static Core.Domain.Contracts.ISpecification<T> ToContractSpec<T>(
            this Core.Common.Specifications.ISpecification<T> spec)
        {
            return Core.Domain.Contracts.SpecificationExtensions.ToContractSpec(spec);
        }
    }
} 