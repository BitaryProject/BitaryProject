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
using Microsoft.Extensions.DependencyInjection;
using Domain.Contracts;
using Domain.Entities.DoctorEntites;
using Services.Abstractions;

namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> userManager;
        private readonly IOptions<JwtOptions> options;
        private readonly IOptions<DomainSettings> domainOptions;
        private readonly IMapper mapper;
        private readonly IMailingService mailingService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IDoctorService _doctorService;

        public AuthenticationService(
            UserManager<User> userManager, 
            IOptions<JwtOptions> options, 
            IOptions<DomainSettings> domainOptions, 
            IMapper mapper, 
            IMailingService mailingService,
            RoleManager<IdentityRole> roleManager,
            IDoctorService doctorService)
        {
            this.userManager = userManager;
            this.options = options;
            this.domainOptions = domainOptions;
            this.mapper = mapper;
            this.mailingService = mailingService;
            this.roleManager = roleManager;
            _doctorService = doctorService;
        }

        public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
        {
            try
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

                // Fix for UserRole=0: Ensure it's mapped to PetOwner (1)
                if ((byte)registerModel.UserRole == 0)
                {
                    registerModel = registerModel with { UserRole = Role.PetOwner };
                }

                var user = new User
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    DisplayName = $"{registerModel.FirstName} {registerModel.LastName}",
                    Email = registerModel.Email,
                    PhoneNumber = registerModel.PhoneNumber,
                    UserName = registerModel.UserName,
                    Gender = registerModel.Gender,
                    UserRole = registerModel.UserRole,
                    EmailConfirmed = false // Explicitly set email confirmation status
                };
                
                // Set default security values
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                
                // Include a direct database approach as a fallback if Identity fails
                try
                {
                    var result = await userManager.CreateAsync(user, registerModel.Password);
                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.Select(e => e.Description).ToList();
                        throw new ValidationException(errors);
                    }
                }
                catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("Discriminator") == true)
                {
                    // Handle discriminator issue with direct intervention
                    throw new Exception("User creation failed due to Discriminator issue. Please contact support.", dbEx);
                }
                
                // Add the user to the appropriate role based on UserRole property
                string roleName = user.UserRole.ToString();
                var roleExists = await userManager.IsInRoleAsync(user, roleName);
                if (!roleExists)
                {
                    await userManager.AddToRoleAsync(user, roleName);
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
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in RegisterAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                // Re-throw to be handled by the controller
                throw;
            }
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
            try
            {
                Console.WriteLine($"LoginAsync called for: {loginModel.Email}");
                
                // First try to safely get the user with error handling to catch SQL NULL issues
                User user;
                try
                {
                    user = await userManager.FindByEmailAsync(loginModel.Email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in FindByEmailAsync: {ex.Message}");
                    Console.WriteLine("Attempting to fix the user account...");
                    
                    // Use UserManager operations to fix user account instead of SQL
                    await SafeFixUserAccount(loginModel.Email);
                    
                    // Try fetching again
                    user = await userManager.FindByEmailAsync(loginModel.Email);
                }
                
                if (user == null)
                {
                    Console.WriteLine($"User not found: {loginModel.Email}");
                    throw new UnAuthorizedException("Email doesn't exist");
                }
                
                Console.WriteLine($"User found: {user.Id}, {user.Email}, UserRole={(int)user.UserRole}");
                
                // Check password
                var passwordValid = await userManager.CheckPasswordAsync(user, loginModel.Password);
                if (!passwordValid)
                {
                    Console.WriteLine($"Invalid password for user: {loginModel.Email}");
                    throw new UnAuthorizedException("Invalid password");
                }
                
                Console.WriteLine($"Password validated for: {loginModel.Email}");
                
                // If user has Admin role (either in UserRole enum or ASP.NET Identity), ensure both are set correctly
                if (user.UserRole == Role.Admin || (await userManager.GetRolesAsync(user)).Contains("Admin"))
                {
                    await EnsureAdminRolesConsistency(user);
                }
                
                // Create token
                try
                {
                    Console.WriteLine("Generating token...");
                    var token = await CreateTokenAsync(user);
                    Console.WriteLine("Token generated successfully");
                    
                    return new UserResultDTO(user.DisplayName, user.Email, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating token: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
            catch (Exception ex) when (!(ex is UnAuthorizedException))
            {
                Console.WriteLine($"Unexpected error in LoginAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        // Safer method to fix user accounts that relies only on UserManager
        private async Task SafeFixUserAccount(string email)
        {
            try
            {
                Console.WriteLine($"Attempting to fix user account: {email}");
                
                // Create a normalized version of the email
                var normalizedEmail = email.ToUpperInvariant();
                
                // Try to find by normalized email (which is how Identity stores emails)
                var users = await userManager.Users
                    .Where(u => u.NormalizedEmail == normalizedEmail)
                    .ToListAsync();
                
                if (users.Count == 0)
                {
                    Console.WriteLine($"No users found with email: {email}");
                    return;
                }
                
                foreach (var user in users)
                {
                    Console.WriteLine($"Fixing user: {user.Id}, {user.Email}");
                    
                    // Ensure all string properties are not null
                    user.UserName = user.UserName ?? $"user_{Guid.NewGuid():N}";
                    user.NormalizedUserName = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();
                    user.Email = user.Email ?? email;
                    user.NormalizedEmail = user.NormalizedEmail ?? normalizedEmail;
                    user.PhoneNumber = user.PhoneNumber ?? "";
                    user.SecurityStamp = user.SecurityStamp ?? Guid.NewGuid().ToString();
                    user.ConcurrencyStamp = user.ConcurrencyStamp ?? Guid.NewGuid().ToString();
                    user.FirstName = user.FirstName ?? "";
                    user.LastName = user.LastName ?? "";
                    user.DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}".Trim();
                    
                    // Set UserRole to a valid value if it's 0 (uninitialized)
                    if ((int)user.UserRole == 0)
                    {
                        user.UserRole = Role.PetOwner;
                    }
                    
                    // Update the user with fixes
                    await userManager.UpdateAsync(user);
                    Console.WriteLine($"User fixed: {user.Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fixing user account: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                // Don't throw, allow the login process to continue
            }
        }

        // Helper method to ensure admin roles are consistent
        private async Task EnsureAdminRolesConsistency(User user)
        {
            try
            {
                Console.WriteLine($"Ensuring admin roles consistency for user: {user.Email}");
                bool needsUpdate = false;
                
                // Ensure UserRole enum is set to Admin
                if (user.UserRole != Role.Admin)
                {
                    Console.WriteLine("Setting UserRole enum to Admin");
                    user.UserRole = Role.Admin;
                    needsUpdate = true;
                }
                
                // Ensure user is in ASP.NET Identity Admin role
                var userRoles = await userManager.GetRolesAsync(user);
                if (!userRoles.Contains("Admin"))
                {
                    Console.WriteLine("Adding user to ASP.NET Identity Admin role");
                    
                    // Ensure Admin role exists
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        Console.WriteLine("Creating Admin role");
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    
                    // Add user to role
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                
                // Update user if needed
                if (needsUpdate)
                {
                    Console.WriteLine("Updating user");
                    await userManager.UpdateAsync(user);
                }
                
                Console.WriteLine("Admin roles consistency ensured");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring admin roles consistency: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                // Don't throw - we don't want to break the login flow
            }
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
            try
            {
                Console.WriteLine($"Creating token for user: {user.Email}, UserRole={(int)user.UserRole}");
                
                var jwtOptions = options.Value;
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };
                
                // Handle UserRole enum - if the value is invalid or not initialized
                try
                {
                    // First check if UserRole is not initialized (it's zero)
                    if ((int)user.UserRole == 0)
                    {
                        Console.WriteLine("UserRole is 0 (not initialized). Setting to PetOwner (1)");
                        user.UserRole = Role.PetOwner;
                        await userManager.UpdateAsync(user);
                    }
                    
                    // Get the name of the role from the enum
                    string userRoleString = user.UserRole.ToString();
                    Console.WriteLine($"User role from enum: {userRoleString}");
                    
                    // Add UserRole as claim
                    authClaims.Add(new Claim(ClaimTypes.Role, userRoleString));
                    
                    // Check if user is a doctor and add DoctorId claim if so
                    if (user.UserRole == Role.Doctor)
                    {
                        Console.WriteLine("User is a doctor, attempting to add DoctorId claim");
                        
                        // Use the doctor service to find the doctor
                        var doctor = await _doctorService.GetDoctorByUserIdAsync(user.Id);
                        
                        if (doctor != null)
                        {
                            Console.WriteLine($"Found doctor with ID: {doctor.Id}");
                            authClaims.Add(new Claim("DoctorId", doctor.Id.ToString()));
                        }
                        else
                        {
                            Console.WriteLine("No doctor profile found for this user");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing UserRole: {ex.Message}");
                    
                    // Set a default UserRole as PetOwner (value 1)
                    user.UserRole = Role.PetOwner;
                    await userManager.UpdateAsync(user);
                    
                    // Still add the role claim
                    authClaims.Add(new Claim(ClaimTypes.Role, Role.PetOwner.ToString()));
                }

                // Get ASP.NET Identity roles
                var roles = await userManager.GetRolesAsync(user);
                Console.WriteLine($"User ASP.NET Identity roles: {string.Join(", ", roles)}");
                
                // Add ASP.NET Identity roles to claims
                authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                // Create and sign token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
                var signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtOptions.Issuer,
                    audience: jwtOptions.Audience,
                    expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                    claims: authClaims,
                    signingCredentials: signingCreds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                Console.WriteLine("Token created successfully");
                
                return tokenString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating token: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
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
                .AsNoTracking() // Ensure we get a fresh copy from the database
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            // Set default values for null fields to avoid sending nulls to client
            var firstName = !string.IsNullOrWhiteSpace(user.FirstName) ? user.FirstName : string.Empty;
            var lastName = !string.IsNullOrWhiteSpace(user.LastName) ? user.LastName : string.Empty;
            var phoneNumber = !string.IsNullOrWhiteSpace(user.PhoneNumber) ? user.PhoneNumber : string.Empty;

            // Initialize an empty address if user doesn't have one
            var address = user.Address != null 
                ? new AddressDTO 
                { 
                    Name = !string.IsNullOrWhiteSpace(user.Address.Name) ? user.Address.Name : string.Empty,
                    Street = !string.IsNullOrWhiteSpace(user.Address.Street) ? user.Address.Street : string.Empty,
                    City = !string.IsNullOrWhiteSpace(user.Address.City) ? user.Address.City : string.Empty,
                    Country = !string.IsNullOrWhiteSpace(user.Address.Country) ? user.Address.Country : string.Empty
                }
                : new AddressDTO 
                { 
                    Name = string.Empty,
                    Street = string.Empty,
                    City = string.Empty,
                    Country = string.Empty
                };

            // Ensure user role is valid (default to PetOwner if it's 0 or invalid)
            var userRole = user.UserRole;
            if ((byte)userRole == 0 || !Enum.IsDefined(typeof(Role), userRole))
            {
                userRole = Role.PetOwner;
            }

            return new UserInformationDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Gender = user.Gender,
                UserRole = userRole,
                Address = address,
                PhoneNumber = phoneNumber
            };
        }

        public async Task UpdateUserInfo(UserInformationDTO userInfoDTO, string email)
        {
            try
            {
                // Get the user - simply using FindByEmailAsync to avoid the tracking issue
                var user = await userManager.FindByEmailAsync(email)
                    ?? throw new UserNotFoundException(email);

                Console.WriteLine($"Found user: ID={user.Id}, Email={user.Email}");

                // Fix for UserRole=0: Ensure it's mapped to PetOwner (1)
                if ((byte)userInfoDTO.UserRole == 0 || !Enum.IsDefined(typeof(Role), userInfoDTO.UserRole))
                {
                    userInfoDTO.UserRole = Role.PetOwner;
                }

                // Copy only the fields we want to update
                user.FirstName = !string.IsNullOrWhiteSpace(userInfoDTO.FirstName) ? userInfoDTO.FirstName : user.FirstName;
                user.LastName = !string.IsNullOrWhiteSpace(userInfoDTO.LastName) ? userInfoDTO.LastName : user.LastName;
                user.Gender = userInfoDTO.Gender;
                user.UserRole = userInfoDTO.UserRole;
                user.PhoneNumber = !string.IsNullOrWhiteSpace(userInfoDTO.PhoneNumber) ? userInfoDTO.PhoneNumber : user.PhoneNumber;
                user.DisplayName = $"{user.FirstName} {user.LastName}".Trim();

                Console.WriteLine($"Updating user: FirstName={user.FirstName}, LastName={user.LastName}, Gender={user.Gender}, Role={user.UserRole}, Phone={user.PhoneNumber}");

                // First, update the user information
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to update user: {errors}");
                }

                Console.WriteLine("User updated successfully");

                // Handle the address using separate methods to avoid tracking conflicts
                if (userInfoDTO.Address != null)
                {
                    await UpdateUserAddressFromUserInfo(userInfoDTO.Address, email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in UpdateUserInfo: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Re-throw the exception after logging
            }
        }

        private async Task UpdateUserAddressFromUserInfo(AddressDTO addressDTO, string email)
        {
            try
            {
                // Create separate db context query to avoid tracking conflicts
                var userWithAddress = await userManager.Users
                    .AsNoTracking() // Important - don't track this entity
                    .Include(u => u.Address)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (userWithAddress == null)
                    throw new UserNotFoundException(email);

                Console.WriteLine($"Address update: User has address: {userWithAddress.Address != null}");

                // Get fresh User instance for update
                var user = await userManager.FindByIdAsync(userWithAddress.Id);
                if (user == null)
                    throw new UserNotFoundException(email);

                if (userWithAddress.Address == null)
                {
                    // Create a new address
                    Console.WriteLine("Creating new address for user");
                    
                    // Map the DTO to a new address entity
                    var newAddress = new UserAddress
                    {
                        UserId = user.Id,
                        Name = addressDTO.Name ?? string.Empty,
                        Street = addressDTO.Street ?? string.Empty,
                        City = addressDTO.City ?? string.Empty,
                        Country = addressDTO.Country ?? string.Empty
                    };
                    
                    // Assign the address to the user
                    user.Address = newAddress;
                }
                else
                {
                    // Create a new address entity with same ID but updated values
                    var updatedAddress = new UserAddress
                    {
                        Id = userWithAddress.Address.Id,
                        UserId = user.Id,
                        Name = addressDTO.Name ?? userWithAddress.Address.Name,
                        Street = addressDTO.Street ?? userWithAddress.Address.Street,
                        City = addressDTO.City ?? userWithAddress.Address.City,
                        Country = addressDTO.Country ?? userWithAddress.Address.Country
                    };
                    
                    // Assign the updated address
                    user.Address = updatedAddress;
                }
                
                // Update the user with the new/updated address
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to update user with address: {errors}");
                }
                
                Console.WriteLine("Address updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating address in UpdateUserAddressFromUserInfo: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                
                // Rethrow to propagate the error
                throw new Exception($"Failed to update address: {ex.Message}", ex);
            }
        }

        public async Task<object> GetDebugInfo(string email)
        {
            var userWithAddress = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (userWithAddress == null)
                throw new UserNotFoundException(email);
            
            var addressData = userWithAddress.Address != null
                ? new
                {
                    AddressId = userWithAddress.Address.Id,
                    UserId = userWithAddress.Address.UserId,
                    Name = userWithAddress.Address.Name,
                    Street = userWithAddress.Address.Street,
                    City = userWithAddress.Address.City,
                    Country = userWithAddress.Address.Country
                }
                : null;
            
            var userRaw = await userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
            
            return new
            {
                User = new
                {
                    Id = userWithAddress.Id,
                    Email = userWithAddress.Email,
                    FirstName = userWithAddress.FirstName,
                    LastName = userWithAddress.LastName,
                    HasAddress = userWithAddress.Address != null
                },
                Address = addressData,
                RawNavigation = userRaw.Address != null // Check if Address navigation property is loaded
            };
        }

        public async Task<bool> FixAdminRoles(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);
                
                // Make sure the UserRole property is set to Admin
                user.UserRole = Role.Admin;
                
                // Update the user with the correct role enum
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to update user role: {errorMessage}");
                }
                
                // Remove user from any existing roles
                var existingRoles = await userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(user, existingRoles);
                }
                
                // Check if Admin role exists
                var roleExists = await roleManager.RoleExistsAsync("Admin");
                if (!roleExists)
                {
                    // Create Admin role if it doesn't exist
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                
                // Add to Admin role
                var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to add user to Admin role: {errorMessage}");
                }
                
                // Return success
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error fixing admin roles: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}