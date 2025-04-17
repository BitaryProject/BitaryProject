using System.Linq;
using Microsoft.EntityFrameworkCore;
using Core.Common.Specifications;

namespace Infrastructure.Persistence.Repositories
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, Core.Common.Specifications.ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply criteria
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply includes
            query = specification.Includes.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Apply include strings
            query = specification.IncludeStrings.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
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
