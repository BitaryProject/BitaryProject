using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public abstract class Specifications<T> where T : class
    {

        protected Specifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>>? Criteria { get; }

        public List<Expression<Func<T, object>>> IncludeExpressions { get; } = new();
        public List<string> StringIncludes { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }


        public int Take { get; private set; }
        public int Skip { get; private set; }

        public bool IsPaginated { get; private set; }
        protected void setOrderBy(Expression<Func<T, object>> expression)
            => OrderBy = expression;
        protected void setOrderByDescending(Expression<Func<T, object>> expression)
            => OrderByDescending = expression;


        protected void AddInclude(Expression<Func<T, object>> expression)
            => IncludeExpressions.Add(expression);
            
        protected void AddInclude(string includePath)
            => StringIncludes.Add(includePath);

        protected void ApplyPagination(int pageIndex, int pageSize)
        {
            IsPaginated = true;
            Take = pageSize;

            Skip = (pageIndex - 1) * pageSize;
        }
    }
}
