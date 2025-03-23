using Domain.Entities.Identity;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

public class CustomEmailTokenProvider<TUser>(IdentityContext _context) : IUserTwoFactorTokenProvider<TUser> where TUser : IdentityUser
{
  

    /// <summary>
    /// Determines whether this provider can generate an OTP token.
    /// </summary>
    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Generates a 5-digit OTP and stores it in the database with an expiration time.
    /// </summary>
    public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
        var otpCode = new Random().Next(10000, 99999).ToString();
        var expiryTime = DateTime.UtcNow.AddMinutes(5); // OTP expires in 5 minutes

        // Remove any existing OTP for the user before generating a new one
        var existingOTP = _context.UserOTPs.FirstOrDefault(o => o.UserId == user.Id);
        if (existingOTP != null)
        {
            _context.UserOTPs.Remove(existingOTP);
        }

        // Create a new OTP record and store it in the database
        var newOTP = new UserOTP
        {
            UserId = user.Id,
            OTP = otpCode,
            Expire = expiryTime
        };

        _context.UserOTPs.Add(newOTP);
        await _context.SaveChangesAsync();

        return otpCode;
    }

    /// <summary>
    /// Validates the OTP entered by the user.
    /// </summary>
    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
    {
        // Retrieve the stored OTP for the user
        var storedOTP = _context.UserOTPs.FirstOrDefault(o => o.UserId == user.Id);

        // If no OTP is found or it's expired, return false
        if (storedOTP == null || DateTime.UtcNow > storedOTP.Expire)
        {
            return false;
        }

        // If OTP matches, remove it from the database (one-time use) and return true
        if (storedOTP.OTP == token)
        {
            _context.UserOTPs.Remove(storedOTP);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}