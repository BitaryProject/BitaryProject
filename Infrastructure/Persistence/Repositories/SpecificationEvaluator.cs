using Core.Common.Specifications;

using System.Linq;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class SpecificationEvaluator<T> where T : class
    {
        // Method for Core.Common.Specifications.ISpecification
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, Core.Common.Specifications.ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply criteria if any
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Include related entities
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Include related entities via string path
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply additional ordering
            if (specification.ThenOrderBy != null)
            {
                query = ((IOrderedQueryable<T>)query).ThenBy(specification.ThenOrderBy);
            }
            else if (specification.ThenOrderByDescending != null)
            {
                query = ((IOrderedQueryable<T>)query).ThenOrderByDescending(specification.ThenOrderByDescending);
            }

            // Apply grouping
            if (specification.GroupBy != null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            // Apply paging
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                             .Take(specification.Take);
            }

            return query;
        }
    }
}









