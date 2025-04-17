using System;
using System.Security.Claims;

namespace Infrastructure.Presentation.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                throw new InvalidOperationException("User ID claim not found");
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }

        public static bool IsInRole(this ClaimsPrincipal user, string role)
        {
            return user.HasClaim(ClaimTypes.Role, role);
        }
    }
} 