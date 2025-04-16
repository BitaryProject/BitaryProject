using System;

namespace Shared.SecurityModels
{
    /// <summary>
    /// Data transfer object for password reset requests
    /// </summary>
    public class PasswordResetRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Data transfer object for submitting a password reset
    /// </summary>
    public class PasswordResetDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
} 