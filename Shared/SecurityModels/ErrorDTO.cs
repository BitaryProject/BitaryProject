using System;
using System.Collections.Generic;

namespace Shared.SecurityModels
{
    /// <summary>
    /// Data transfer object for error responses
    /// </summary>
    public class ErrorDTO
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Data transfer object for validation errors
    /// </summary>
    public class ValidationErrorDTO : ErrorDTO
    {
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Data transfer object for user role information
    /// </summary>
    public class UserRoleDTO
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
} 