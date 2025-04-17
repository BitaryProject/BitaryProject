using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Implementations.Infrastructure.Repositories
{
    /// <summary>
    /// A generic mock repository that can be used as a placeholder for testing
    /// </summary>
    public class MockRepository<T> 
    {
        public Task<bool> AnyAsync()
        {
            return Task.FromResult(false);
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(0);
        }
    }
} 