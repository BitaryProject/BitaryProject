using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Common.Specifications;

namespace Domain.Contracts
{
    public class Specifications<T> : ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        public Specifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Specifications()
        {
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        public void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        public void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        public void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            if (Criteria != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var body = Expression.AndAlso(
                    Expression.Invoke(Criteria, parameter),
                    Expression.Invoke(criteria, parameter));
                Criteria = Expression.Lambda<Func<T, bool>>(body, parameter);
            }
            else
            {
                Criteria = criteria;
            }
        }
    }
} 