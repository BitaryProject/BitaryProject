using System;
using System.Runtime.CompilerServices;
using Core.Common.Specifications;

namespace Core.Common.Specifications
{
    /// <summary>
    /// This class exists to help with resolving ISpecification interface
    /// to avoid circular dependencies between assemblies.
    /// </summary>
    public static class SpecificationTypeForwarding
    {
        // This class is intentionally empty as it serves only as a marker
        // for type resolution across assemblies.
    }
} 