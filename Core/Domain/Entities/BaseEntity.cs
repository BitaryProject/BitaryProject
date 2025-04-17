using System;

namespace Core.Domain.Entities
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}

