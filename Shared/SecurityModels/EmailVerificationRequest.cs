using System;

namespace Shared.SecurityModels
{
    /// <summary>
    /// Data transfer object for email verification requests
    /// </summary>
    public class EmailVerificationRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }

    /// <summary>
    /// Data transfer object for submitting an email verification code
    /// </summary>
    public class EmailVerificationDTO
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }
} 