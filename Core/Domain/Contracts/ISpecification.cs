using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain.Contracts
{
    /// <summary>
    /// Interface for specification pattern implementation
    /// </summary>
    /// <typeparam name="T">Type of entity this specification applies to</typeparam>
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        Expression<Func<T, object>> GroupBy { get; }

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
        
        /// <summary>
        /// Adds a criteria to the specification
        /// </summary>
        void AddCriteria(Expression<Func<T, bool>> criteria);
    }
} 