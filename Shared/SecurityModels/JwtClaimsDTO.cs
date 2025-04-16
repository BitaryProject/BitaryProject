using System;

namespace Shared.SecurityModels
{
    /// <summary>
    /// Data transfer object for JWT token claims
    /// </summary>
    public class JwtClaimsDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
} 