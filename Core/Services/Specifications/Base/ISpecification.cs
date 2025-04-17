using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

// Forward type to avoid breaking existing code
[assembly: TypeForwardedTo(typeof(Core.Common.Specifications.ISpecification<>))]

namespace Core.Services.Specifications.Base
{
    /// <summary>
    /// This file is being kept to maintain backward compatibility.
    /// All code should use Core.Common.Specifications.ISpecification<T> directly.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    [Obsolete("Use Core.Common.Specifications.ISpecification<T> instead", false)]
    public interface ISpecification<T> : Core.Common.Specifications.ISpecification<T>
    {
        // This interface intentionally inherits all members from Core.Common.Specifications.ISpecification<T>
    }
} 
