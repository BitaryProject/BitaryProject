using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    /// <summary>
    /// Data transfer object for paged results
    /// </summary>
    /// <typeparam name="T">The type of items in the page</typeparam>
    public class PagedResultDTO<T>
    {
        /// <summary>
        /// The items in the current page
        /// </summary>
        public List<T> Items { get; set; }
        
        /// <summary>
        /// The total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// The current page index (1-based)
        /// </summary>
        public int PageIndex { get; set; }
        
        /// <summary>
        /// The number of items per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        
        /// <summary>
        /// Creates a new instance of PagedResultDTO
        /// </summary>
        public PagedResultDTO()
        {
            Items = new List<T>();
        }
        
        /// <summary>
        /// Creates a new instance of PagedResultDTO with the specified items and metadata
        /// </summary>
        public PagedResultDTO(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = new List<T>(items);
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
} 