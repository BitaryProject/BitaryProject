﻿using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using AutoMapper;
using Shared.OrderModels;
using UserAddress = Domain.Entities.SecurityEntities.Address;

namespace Services
{
    public class AuthenticationService(
        UserManager<User> userManager,
        IOptions<JwtOptions> options,
        IOptions<DomainSettings> domainOptions,
        IMapper mapper,
        IMailingService mailingService) : IAuthenticationService
    {
        public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
        {
            // Validate input parameters
            List<string> validationErrors = new();
            
            if (string.IsNullOrWhiteSpace(registerModel.Email))
                validationErrors.Add("Email is required");
            
            if (string.IsNullOrWhiteSpace(registerModel.UserName))
                validationErrors.Add("Username is required");
            
            if (string.IsNullOrWhiteSpace(registerModel.Password))
                validationErrors.Add("Password is required");
            
            if (string.IsNullOrWhiteSpace(registerModel.FirstName) || string.IsNullOrWhiteSpace(registerModel.LastName))
                validationErrors.Add("First name and last name are required");
            
            if (!string.IsNullOrWhiteSpace(registerModel.Email) && await userManager.FindByEmailAsync(registerModel.Email) != null)
                validationErrors.Add("Email is already in use");
            
            if (!string.IsNullOrWhiteSpace(registerModel.UserName) && await userManager.FindByNameAsync(registerModel.UserName) != null) 
                validationErrors.Add("Username is already in use");
            
            if (validationErrors.Any())
                throw new ValidationException(validationErrors);

            var user = new User
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                DisplayName = $"{registerModel.FirstName} {registerModel.LastName}",
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
                UserName = registerModel.UserName,
                Gender = registerModel.Gender
            };

            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }
            
            try
            {
                // Generate and send verification code if registration is successful
                var DomainOptions = domainOptions.Value;
                var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");

                await mailingService.SendEmailAsync(user.Email!, "Verification Code", 
                    $"{DomainOptions.bitaryUrl}api/Authentication/VerifyEmail?email={registerModel.Email}&otp={verificationCode}");
            }
            catch (Exception)
            {
                // Even if email sending fails, continue returning the user
            }

            return new UserResultDTO(
                user.DisplayName,
                user.Email,
                await CreateTokenAsync(user));
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            return await userManager.FindByEmailAsync(email) != null;
        }
        public async Task<AddressDTO> AddUserAddress(AddressDTO address, string email)
        {
            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            if (user.Address != null)
            {
                throw new Exception("User already has an address. Use update endpoint instead.");
            }

            user.Address = mapper.Map<UserAddress>(address);

            await userManager.UpdateAsync(user);
            return mapper.Map<AddressDTO>(user.Address);
        }

        public async Task<AddressDTO> GetUserAddress(string email)
        {
            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            return user.Address != null ? mapper.Map<AddressDTO>(user.Address) : throw new Exception("User address not found.");
        }

        public async Task<UserResultDTO> GetUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);

            return new UserResultDTO(user.DisplayName, user.Email, await CreateTokenAsync(user));
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new UnauthorizedAccessException("User not found");

            var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");



            var DomainOptions = domainOptions.Value;

            await mailingService.SendEmailAsync(user.Email!, "Verification Code", $"{DomainOptions.bitaryUrl}api/Authentication/VerifyEmail?email={email}&otp={verificationCode}");

            return true;
        }

        public async Task<bool> SendResetPasswordEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            var user = await userManager.FindByEmailAsync(email);
            if (user == null) 
                throw new UserNotFoundException(email);

            try
            {
                // Update security stamp to invalidate any previous tokens
                await userManager.UpdateSecurityStampAsync(user);

                // Generate a new password reset token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                
                // URL encode the token to ensure it's properly formatted in the URL
                var encodedToken = Uri.EscapeDataString(token);
                
                var DomainOptions = domainOptions.Value;
                var resetLink = $"{DomainOptions.bitaryUrl}api/Authentication/ResetPassword?email={Uri.EscapeDataString(email)}&token={encodedToken}";

                // Send the reset password email with the generated link
                await mailingService.SendEmailAsync(
                    user.Email!, 
                    "Reset Password", 
                    $"Click the link to reset your password: {resetLink}\n\n" +
                    "This link will expire in 24 hours. If you did not request this password reset, please ignore this email."
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send reset password email: {ex.Message}", ex);
            }
        }
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));
            
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Reset token is required", nameof(token));
            
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required", nameof(newPassword));

            var user = await userManager.FindByEmailAsync(email);
            if (user == null) 
                throw new UserNotFoundException(email);

            try
            {
                // First verify the token is valid
                var isValidToken = await userManager.VerifyUserTokenAsync(
                    user, 
                    TokenOptions.DefaultProvider, 
                    "ResetPassword", 
                    token
                );

                if (!isValidToken)
                {
                    // Try to generate a new token to check if the user's security stamp has changed
                    await userManager.UpdateSecurityStampAsync(user);
                    var newToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    
                    // If we can generate a new token but the provided one is invalid, it's likely expired
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        throw new Exception("Password reset token has expired. Please request a new password reset link.");
                    }
                    
                    throw new Exception("Invalid password reset token. Please request a new password reset link.");
                }

                var resetResult = await userManager.ResetPasswordAsync(user, token, newPassword);
                if (!resetResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    throw new Exception($"Password reset failed: {errorMessage}");
                }

                // Update security stamp after successful password reset
                await userManager.UpdateSecurityStampAsync(user);
                return true;
            }
            catch (Exception ex) when (ex is not UserNotFoundException)
            {
                throw new Exception($"Error resetting password: {ex.Message}", ex);
            }
        }



        public async Task<UserResultDTO> LoginAsync(LoginDTO loginModel)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email) ?? throw new UnAuthorizedException("Email doesn't exist");
            if (!await userManager.CheckPasswordAsync(user, loginModel.Password))
                throw new UnAuthorizedException("Invalid password");

            return new UserResultDTO(user.DisplayName, user.Email, await CreateTokenAsync(user));
        }

        public async Task<AddressDTO> UpdateUserAddress(AddressDTO address, string email)
        {
            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            if (user.Address == null)
            {
                user.Address = mapper.Map<UserAddress>(address);
            }
            else
            {
                user.Address.Name = address.Name;
                user.Address.Street = address.Street;
                user.Address.City = address.City;
                user.Address.Country = address.Country;
            }

            await userManager.UpdateAsync(user);
            return mapper.Map<AddressDTO>(user.Address);
        }
     
        private async Task<string> CreateTokenAsync(User user)
        {
            var jwtOptions = options.Value;
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                //new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var roles = await userManager.GetRolesAsync(user);
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            var signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                claims: authClaims,
                signingCredentials: signingCreds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> VerifyEmailAsync(string email, string otp)
        {
            var user = await userManager.FindByEmailAsync(email) ?? throw new UnauthorizedAccessException("User not found");
            if (!await userManager.VerifyUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation", otp))
                throw new Exception("Invalid or expired verification code");

            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);
            var changeResult = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changeResult.Succeeded)
                throw new Exception(string.Join(", ", changeResult.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<UserInformationDTO> GetUserInfo(string email)
        {
            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            // Initialize an empty address if user doesn't have one
            var address = user.Address != null 
                ? mapper.Map<AddressDTO>(user.Address) 
                : new AddressDTO 
                { 
                    City = string.Empty,
                    Country = string.Empty,
                    Street = string.Empty,
                    Name = string.Empty
                };

            return new UserInformationDTO
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Gender = user.Gender,
                Address = address
            };
        }

        public async Task UpdateUserInfo(UserInformationDTO userInfoDTO, string email, AddressDTO address)
        {
            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            user.FirstName = userInfoDTO.FirstName;
            user.LastName = userInfoDTO.LastName;
            user.Gender = userInfoDTO.Gender;

            if (user.Address == null)
            {
                user.Address = mapper.Map<UserAddress>(address);
            }
            else
            {
                user.Address.Name = address.Name;
                user.Address.Street = address.Street;
                user.Address.City = address.City;
                user.Address.Country = address.Country;
            }

            await userManager.UpdateAsync(user);
        }
    }
}
//using Domain.Entities.SecurityEntities;
//using MailKit;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using Shared.OrderModels;
//using Shared.SecurityModels;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using UserAddress = Domain.Entities.SecurityEntities.Address;
//using Domain.Exceptions;



//namespace Services
//{
//    public class AuthenticationService(
//        UserManager<User> userManager,
//        IOptions<JwtOptions> options,
//        IOptions<DomainSettings> domainOptions
//        ,IMapper mapper,
//        IMailingService mailingService) : IAuthenticationService
//    {
//        public async Task<bool> CheckEmailExist(string email)
//        {
//            var user = await userManager.FindByEmailAsync(email);

//            return user != null;
//        }

//        public async Task<AddressDTO> GetUserAddress(string email)
//        {
//            var user =await userManager.Users
//                .Include(u=>u.Address).FirstOrDefaultAsync(u=>u.Email ==email)
//                ?? throw new UserNotFoundException(email);

//            return mapper.Map<AddressDTO>(user.Address);
//            //return new AddressDTO
//            //{
//            //    City=user.Address.City,
//            //    Country=user.Address.Country,
//            //    FirstName=user.Address.FirstName,
//            //    LastName=user.Address.LastName
//            //};
//        }

//        public async Task<UserResultDTO> GetUserByEmail(string email)
//        {
//            var user = await userManager.FindByEmailAsync(email)??throw new UserNotFoundException(email);

//            return new UserResultDTO(user.DisplayName,
//                         user.Email,
//                          await CreateTokenAsync(user));
//        }

//        public async Task<UserResultDTO> LoginAsync(LoginDTO loginModel)
//        {
//            var user= await userManager.FindByEmailAsync(loginModel.Email);
//            if (user == null)
//                throw new UnAuthorizedException("Email Doesn't Exist");
//            var result = await userManager.CheckPasswordAsync(user, loginModel.Password);
//            if(!result)
//                throw new UnAuthorizedException(); 

//            return new UserResultDTO(user.DisplayName,
//                user.Email,
//                 await CreateTokenAsync(user));

//        }

//        //public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
//        //{
//        //    var user = new User
//        //    {

//        //        DisplayName = registerModel.DisplayName,
//        //        Email = registerModel.Email,
//        //        PhoneNumber = registerModel.PhoneNumber,
//        //        UserName = registerModel.UserName,
//        //    };

//        //    var result = await userManager.CreateAsync(user, registerModel.Password);
//        //    if (!result.Succeeded)
//        //    {
//        //        var errors=result.Errors.Select(e=>e.Description).ToList();
//        //        throw new ValidationException(errors);
//        //    }
//        //    return new UserResultDTO(
//        //                user.DisplayName,
//        //                 user.Email,
//        //               await  CreateTokenAsync(user));


//        //}

//        public async Task<AddressDTO> UpdateUserAddress(AddressDTO address,string email)
//        {
//            var user = await userManager.Users
//                           .Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email)
//                           ?? throw new UserNotFoundException(email);


//            if(user.Address != null)
//            {
//                user.Address.FirstName = address.FirstName;
//                user.Address.LastName = address.LastName;
//                user.Address.Street = address.Street;
//                user.Address.City = address.City;
//                user.Address.Country = address.Country;
//            }
//            else
//            {
//                var userAddress=mapper.Map<UserAddress>(address);
//                user.Address = userAddress;
//            }

//            await userManager.UpdateAsync(user);

//            return address;
//        }

//        private async Task<string> CreateTokenAsync(User user)
//        {
//            var jwtOptions = options.Value;

//            var authClaims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name,user.UserName),
//                new Claim(ClaimTypes.Email,user.Email)
//            };

//            //Add role to this user if he has exist role
//            var roles = await userManager.GetRolesAsync(user);

//            foreach (var role in roles)
//            {
//                authClaims.Add(new Claim(ClaimTypes.Role, role));
//            }

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

//            var signingCreds=new SigningCredentials(key ,SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                audience: jwtOptions.Audience,
//                issuer: jwtOptions.Issuer,
//                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
//                claims:authClaims,
//                signingCredentials:signingCreds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
//        {
//            if (await userManager.FindByEmailAsync(registerModel.Email) != null)
//                throw new ValidationException(new List<string> { "Email is already taken." });

//            if (await userManager.FindByNameAsync(registerModel.UserName) != null)
//                throw new ValidationException(new List<string> { "Username is already taken." });

//            var user = new User()
//            {
//                FirstName = registerModel.FirstName,
//                LastName = registerModel.LastName,
//                DisplayName = $"{registerModel.FirstName} {registerModel.LastName}",
//                Email = registerModel.Email,
//                PhoneNumber = registerModel.PhoneNumber,
//                UserName = registerModel.UserName,
//                Gender = registerModel.Gender,
//            };
//            var result = await userManager.CreateAsync(user, registerModel.Password);
//            if (!result.Succeeded)
//            {
//                var errors = result.Errors.Select(e => e.Description).ToList();
//                throw new ValidationException(errors);
//            }
//            var DomainOptions = domainOptions.Value;
//            var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");

//            await mailingService.SendEmailAsync(user.Email!, "Verification Code", $"{DomainOptions.localUrl}api/Authentication/VerifyEmail?email={registerModel.Email}&otp={verificationCode}");



//            return new UserResultDTO(
//               user.DisplayName,
//               user.Email!,
//               await CreateTokenAsync(user));
//        }

//        public async Task<bool> VerifyEmailAsync(string email, string otp)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null) throw new UnauthorizedAccessException("User not found");

//            var isValid = await userManager.VerifyUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation", otp);

//            //var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "EmailConfirmation");


//            if (!isValid) throw new Exception("Invalid or expired verification code");

//            user.EmailConfirmed = true;
//            await userManager.UpdateAsync(user);

//            return true;
//        }


//        public async Task<bool> SendVerificationCodeAsync(string email)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null) throw new UnauthorizedAccessException("User not found");

//            var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");

//            //var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "EmailConfirmation");

//            var DomainOptions = domainOptions.Value;

//            await mailingService.SendEmailAsync(user.Email!, "Verification Code", $"{DomainOptions.localUrl}api/Authentication/VerifyEmail?email={email}&otp={verificationCode}");

//            return true;
//        }

//        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null) throw new UserNotFoundException(email);

//            var changeResult = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
//            if (!changeResult.Succeeded)
//                throw new Exception(string.Join(", ", changeResult.Errors.Select(e => e.Description)));

//            return true;
//        }

//        public async Task<bool> SendResetPasswordEmailAsync(string email)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null) throw new Exception("User not found");

//            // Update the security stamp to invalidate any previous tokens
//            await userManager.UpdateSecurityStampAsync(user);

//            // Generate a new password reset token after updating the security stamp
//            var token = await userManager.GeneratePasswordResetTokenAsync(user);
//            var DomainOptions = domainOptions.Value;

//            var resetLink = $"{DomainOptions.localUrl}api/Authentication/ResetPassword?email={email}&token={token}";

//            // Send the reset password email with the generated link
//            await mailingService.SendEmailAsync(user.Email!, "Reset Password", $"Click the link to reset your password: {resetLink}");

//            return true;
//        }
//        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null) throw new UserNotFoundException(email);

//            var isValidToken = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);
//            if (!isValidToken) throw new Exception("Invalid or expired token");

//            var resetResult = await userManager.ResetPasswordAsync(user, token, newPassword);
//            if (!resetResult.Succeeded)
//                throw new Exception(string.Join(", ", resetResult.Errors.Select(e => e.Description)));

//            return true;
//        }

//        public async Task<UserInformationDTO> GetUserInfo(string email,UserAddress address)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null)
//                return null;

//            return new UserInformationDTO
//            {
//                FirstName = user.FirstName,
//                LastName = user.LastName,
//                Gender = user.Gender,
//                Address = mapper.Map<AddressDTO>(address)
//            };



//        }

//        public async Task UpdateUserInfo(UserInformationDTO userInfoDTO, string email, AddressDTO address)
//        {
//            var user = await userManager.FindByEmailAsync(email);
//            if (user == null) return;

//            user.FirstName = userInfoDTO.FirstName;
//            user.LastName = userInfoDTO.LastName;
//            user.Gender = userInfoDTO.Gender;
//            user.Address = mapper.Map<UserAddress>(address);

//            await userManager.UpdateAsync(user);
//        }
//    }


//}