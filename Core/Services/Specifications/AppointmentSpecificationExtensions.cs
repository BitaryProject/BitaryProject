using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;

namespace Core.Services.Specifications
{
    /// <summary>
    /// Extension methods for working with the AppointmentSpecification
    /// </summary>
    public static class AppointmentSpecificationExtensions
    {
        /// <summary>
        /// Creates a BaseSpecification from an AppointmentSpecification
        /// </summary>
        public static BaseSpecification<Appointment> ToBaseSpecification(this AppointmentSpecification spec)
        {
            // Create new base specification with the criteria
            var baseSpec = new BaseSpecification<Appointment>(spec.Criteria);
            
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
    }
} 