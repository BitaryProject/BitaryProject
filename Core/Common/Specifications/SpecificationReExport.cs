using Core.Common.Specifications;
using System;
using System.Linq.Expressions;

namespace Core.Domain.Contracts
{
    /// <summary>
    /// Re-export of ISpecification interface to avoid circular dependencies
    /// </summary>
    /// <typeparam name="T">The type of entity to which the specification is applied</typeparam>
    public interface ISpecification<T> : Core.Common.Specifications.ISpecification<T>
    {
        // This interface inherits all members from Core.Common.Specifications.ISpecification<T>
    }
} 