using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Common.Specifications;
using Core.Domain.Entities.HealthcareEntities;

namespace Core.Services.Specifications
{
    /// <summary>
    /// Adapter class that adapts AppointmentSpecification to Core.Common.Specifications.ISpecification
    /// for use with the repository system
    /// </summary>
    public class AppointmentSpecificationAdapter : Core.Common.Specifications.ISpecification<Appointment>
    {
        private readonly AppointmentSpecification _specification;

        public AppointmentSpecificationAdapter(AppointmentSpecification specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        public Expression<Func<Appointment, bool>> Criteria => _specification.Criteria;

        public List<Expression<Func<Appointment, object>>> Includes => _specification.Includes;

        public List<string> IncludeStrings => _specification.IncludeStrings;

        public Expression<Func<Appointment, object>> OrderBy => _specification.OrderBy;

        public Expression<Func<Appointment, object>> OrderByDescending => _specification.OrderByDescending;

        public Expression<Func<Appointment, object>> GroupBy => _specification.GroupBy;

        public int Take => _specification.Take;

        public int Skip => _specification.Skip;

        public bool IsPagingEnabled => _specification.IsPagingEnabled;
        
        // Implementation of the required methods
        public void AddCriteria(Expression<Func<Appointment, bool>> criteria)
        {
            // Not supported directly on AppointmentSpecification
            throw new NotSupportedException("Cannot modify criteria directly on an adapter");
        }
        
        public void AddInclude(Expression<Func<Appointment, object>> includeExpression)
        {
            _specification.AddInclude(includeExpression);
        }
        
        public void AddInclude(string includeString)
        {
            _specification.AddInclude(includeString);
        }
        
        public void AddOrderBy(Expression<Func<Appointment, object>> orderByExpression)
        {
            _specification.AddOrderBy(orderByExpression);
        }
        
        public void ApplyOrderBy(Expression<Func<Appointment, object>> orderByExpression)
        {
            _specification.AddOrderBy(orderByExpression);
        }
        
        public void ApplyOrderByDescending(Expression<Func<Appointment, object>> orderByDescendingExpression)
        {
            _specification.AddOrderByDescending(orderByDescendingExpression);
        }
        
        public void ApplyPaging(int skip, int take)
        {
            _specification.ApplyPaging(skip, take);
        }
    }

    /// <summary>
    /// Extension methods for AppointmentSpecification
    /// </summary>
    public static class AppointmentSpecificationExtensions
    {
        /// <summary>
        /// Converts an AppointmentSpecification to a Common.Specifications.ISpecification
        /// </summary>
        public static Core.Common.Specifications.ISpecification<Appointment> ToSpecification(this AppointmentSpecification spec)
        {
            return new AppointmentSpecificationAdapter(spec);
        }
    }
} 