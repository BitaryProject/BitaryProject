using Core.Domain.Entities;
using System;

namespace Core.Domain.Entities.ProductEntities
{
    public class ProductType : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
} 