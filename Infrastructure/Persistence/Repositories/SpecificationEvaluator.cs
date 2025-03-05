﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>
           (IQueryable<T> inputQuery, Specifications<T> specifications) where T : class
        {
            var query = inputQuery;

            if (specifications.Criteria is not null)
                query = query.Where(specifications.Criteria);

            query = specifications.IncludeExpressions.Aggregate(
                query,
                (currentQuery, includeExpressions) => currentQuery.Include(includeExpressions));

            if (specifications.OrderBy is not null)
                query = query.OrderBy(specifications.OrderBy);

            else if (specifications.OrderByDescending is not null)
                query = query.OrderByDescending(specifications.OrderByDescending);

            if (specifications.IsPaginated)
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            return query;
        }
    }
}
