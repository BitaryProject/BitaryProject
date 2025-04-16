using System;
using System.Collections.Generic;

namespace Domain.Entities.HealthcareEntities
{
    /// <summary>
    /// Entity representing a paginated result set
    /// </summary>
    /// <typeparam name="T">Type of items in the paged result</typeparam>
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        public PagedResult()
        {
            Items = new List<T>();
        }
        
        public PagedResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
} 