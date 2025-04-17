using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Common.Specifications
{
    /// <summary>
    /// Specification interface for query specifications
    /// </summary>
    /// <typeparam name="T">Type of entity this specification applies to</typeparam>
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
        
        void AddCriteria(Expression<Func<T, bool>> criteria);
        void AddInclude(Expression<Func<T, object>> includeExpression);
        void AddInclude(string includeString);
        void AddOrderBy(Expression<Func<T, object>> orderByExpression);
        void ApplyOrderBy(Expression<Func<T, object>> orderByExpression);
        void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression);
        void ApplyPaging(int skip, int take);
    }
} 