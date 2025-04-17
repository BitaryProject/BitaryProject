using System;
using System.Collections.Generic;

namespace Shared.HealthcareModels
{
    public class HealthcarePagedResultDTO<T>
    {
        public ICollection<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

        public HealthcarePagedResultDTO(ICollection<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public HealthcarePagedResultDTO()
        {
            Items = new List<T>();
        }
    }
} 