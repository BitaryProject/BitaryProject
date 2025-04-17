using Core.Domain.Entities.HealthcareEntities;
using Core.Services.Specifications.Base;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace Core.Services.Specifications
{
    /// <summary>
    /// Specification for Appointment entity queries
    /// </summary>
    public class AppointmentSpecification
    {
        public AppointmentSpecification(Expression<Func<Appointment, bool>> criteria = null)
        {
            Criteria = criteria;
        }

        public Expression<Func<Appointment, bool>> Criteria { get; private set; }
        public List<Expression<Func<Appointment, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();
        public Expression<Func<Appointment, object>> OrderBy { get; private set; }
        public Expression<Func<Appointment, object>> OrderByDescending { get; private set; }
        public Expression<Func<Appointment, object>> GroupBy { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        /// <summary>
        /// Adds an include expression to the specification
        /// </summary>
        public AppointmentSpecification AddInclude(Expression<Func<Appointment, object>> includeExpression)
        {
            Includes.Add(includeExpression);
            return this;
        }

        /// <summary>
        /// Adds a string-based include to the specification
        /// </summary>
        public AppointmentSpecification AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
            return this;
        }

        /// <summary>
        /// Adds an order by expression to the specification
        /// </summary>
        public AppointmentSpecification AddOrderBy(Expression<Func<Appointment, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            return this;
        }

        /// <summary>
        /// Adds a descending order by expression to the specification
        /// </summary>
        public AppointmentSpecification AddOrderByDescending(Expression<Func<Appointment, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
            return this;
        }

        /// <summary>
        /// Applies paging to the specification
        /// </summary>
        public AppointmentSpecification ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
            return this;
        }
    }
}
