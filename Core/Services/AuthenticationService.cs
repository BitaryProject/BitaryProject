using Core.Domain.Entities.SecurityEntities;
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
using Core.Domain.Exceptions;
using AutoMapper;
using Shared.OrderModels;
using UserAddress = Core.Domain.Entities.SecurityEntities.Address;
using Microsoft.Extensions.DependencyInjection;
using Core.Domain.Entities.HealthcareEntities;
using Microsoft.Extensions.Logging;
using Core.Domain.Contracts;
using Core.Services.Abstractions;

namespace Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<JwtOptions> _options;
        private readonly IOptions<DomainSettings> _domainOptions;
        private readonly IMapper _mapper;
        private readonly IMailingService _mailingService;
        private readonly IHealthcareUnitOfWork _healthcareUnitOfWork;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtOptions> options,
            IOptions<DomainSettings> domainOptions,
            IMapper mapper,
            IMailingService mailingService,
            IHealthcareUnitOfWork healthcareUnitOfWork,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _options = options;
            _domainOptions = domainOptions;
            _mapper = mapper;
            _mailingService = mailingService;
            _healthcareUnitOfWork = healthcareUnitOfWork;
            _logger = logger;
        }

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
            
            if (!string.IsNullOrWhiteSpace(registerModel.Email) && await _userManager.FindByEmailAsync(registerModel.Email) != null)
                validationErrors.Add("Email is already in use");
            
            if (!string.IsNullOrWhiteSpace(registerModel.UserName) && await _userManager.FindByNameAsync(registerModel.UserName) != null) 
                validationErrors.Add("Username is already in use");
            
            if (validationErrors.Any())
                throw new ValidationException(validationErrors);

            // Map the registration DTO to user entity
            var user = _mapper.Map<User>(registerModel);

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }
            
            List<string> assignedRoles = new List<string>();
            
            // Assign a role based on the user type if specified
            if (!string.IsNullOrEmpty(registerModel.UserType))
            {
                // Ensure the role exists
                string role = registerModel.UserType.Trim();
                
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogInformation("Creating new role: {Role}", role);
                    // Create the role if it doesn't exist
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                
                _logger.LogInformation("Assigning role {Role} to user {UserId}", role, user.Id);
                // Assign the role to the user
                await _userManager.AddToRoleAsync(user, role);
                assignedRoles.Add(role);
                
                // Create specific profiles based on the role
                try 
                {
                    switch (role)
                    {
                        case "Doctor":
                            await CreateDoctorProfile(user);
                            break;
                        case "PetOwner":
                            await CreatePetOwnerProfile(user);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue with user creation
                    _logger.LogError(ex, "Failed to create {Role} profile for user {UserId}, but continuing with user creation", 
                        role, user.Id);
                }
            }
            else
            {
                // Default to a basic user role if no specific role is provided
                if (!await _roleManager.RoleExistsAsync("Customer"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                await _userManager.AddToRoleAsync(user, "Customer");
                assignedRoles.Add("Customer");
            }
            
            try
            {
                // Generate and send verification code if registration is successful
                var DomainOptions = _domainOptions.Value;
                var verificationCode = await _userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");

                await _mailingService.SendEmailAsync(user.Email!, "Verification Code", 
                    $"{DomainOptions.bitaryUrl}api/Authentication/VerifyEmail?email={registerModel.Email}&otp={verificationCode}");
            }
            catch (Exception ex)
            {
                // Log email sending error but continue
                _logger.LogError(ex, "Failed to send verification email to user {UserId} with email {Email}", 
                    user.Id, user.Email);
            }

            // Create token
            var token = await CreateTokenAsync(user);
            
            // Return user result DTO
            return new UserResultDTO(
                        user.DisplayName,
                         user.Email,
                token,
                assignedRoles);
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        public async Task<AddressDTO> AddUserAddress(AddressDTO address, string email)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            if (user.Address != null)
            {
                throw new Exception("User already has an address. Use update endpoint instead.");
            }

            user.Address = _mapper.Map<UserAddress>(address);

            await _userManager.UpdateAsync(user);
            return _mapper.Map<AddressDTO>(user.Address);
        }

        public async Task<AddressDTO> GetUserAddress(string email)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            return user.Address != null ? _mapper.Map<AddressDTO>(user.Address) : throw new Exception("User address not found.");
        }

        public async Task<UserResultDTO> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);

            return new UserResultDTO(user.DisplayName, user.Email, await CreateTokenAsync(user));
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new UnauthorizedAccessException("User not found");

            var verificationCode = await _userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");



            var DomainOptions = _domainOptions.Value;

            await _mailingService.SendEmailAsync(user.Email!, "Verification Code", $"{DomainOptions.bitaryUrl}api/Authentication/VerifyEmail?email={email}&otp={verificationCode}");

            return true;
        }

        public async Task<bool> SendResetPasswordEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                throw new UserNotFoundException(email);

            try
            {
                // Update security stamp to invalidate any previous tokens
            await _userManager.UpdateSecurityStampAsync(user);

                // Generate a new password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // URL encode the token to ensure it's properly formatted in the URL
                var encodedToken = Uri.EscapeDataString(token);
                
                var DomainOptions = _domainOptions.Value;
                var resetLink = $"{DomainOptions.bitaryUrl}api/Authentication/ResetPassword?email={Uri.EscapeDataString(email)}&token={encodedToken}";

            // Send the reset password email with the generated link
                await _mailingService.SendEmailAsync(
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

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                throw new UserNotFoundException(email);

            try
            {
                // First verify the token is valid
                var isValidToken = await _userManager.VerifyUserTokenAsync(
                    user, 
                    TokenOptions.DefaultProvider, 
                    "ResetPassword", 
                    token
                );

                if (!isValidToken)
                {
                    // Try to generate a new token to check if the user's security stamp has changed
                    await _userManager.UpdateSecurityStampAsync(user);
                    var newToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    
                    // If we can generate a new token but the provided one is invalid, it's likely expired
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        throw new Exception("Password reset token has expired. Please request a new password reset link.");
                    }
                    
                    throw new Exception("Invalid password reset token. Please request a new password reset link.");
                }

            var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!resetResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    throw new Exception($"Password reset failed: {errorMessage}");
                }

                // Update security stamp after successful password reset
                await _userManager.UpdateSecurityStampAsync(user);
            return true;
            }
            catch (Exception ex) when (ex is not UserNotFoundException)
            {
                throw new Exception($"Error resetting password: {ex.Message}", ex);
            }
        }



        public async Task<UserResultDTO> LoginAsync(LoginDTO loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email) ?? throw new UnAuthorizedException("Email doesn't exist");
            
            if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
                throw new UnAuthorizedException("Invalid password");

            _logger.LogInformation("User {UserId} with email {Email} logged in successfully", user.Id, user.Email);
            
            // Get user roles to include in the response
            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogDebug("User {UserId} has roles: {Roles}", user.Id, string.Join(", ", roles));
            
            // Create JWT token
            var token = await CreateTokenAsync(user);
            
            // Create response using UserResultDTO constructor
            return new UserResultDTO(
                user.DisplayName, 
                user.Email, 
                token,
                roles.ToList());
        }

        public async Task<AddressDTO> UpdateUserAddress(AddressDTO address, string email)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UserNotFoundException(email);

            if (user.Address == null)
            {
                user.Address = _mapper.Map<UserAddress>(address);
            }
            else
            {
                user.Address.Name = address.Name;
                user.Address.Street = address.Street;
                user.Address.City = address.City;
                user.Address.Country = address.Country;
            }

            await _userManager.UpdateAsync(user);
            return _mapper.Map<AddressDTO>(user.Address);
        }
     
        private async Task<string> CreateTokenAsync(User user)
        {
            var jwtOptions = _options.Value;
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Map user to claims DTO to extract necessary claims
            var claimsDTO = _mapper.Map<JwtClaimsDTO>(user);
            
            // Add user ID claim
            authClaims.Add(new Claim(ClaimTypes.NameIdentifier, claimsDTO.Id));

            // Add role claims
            var roles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _logger.LogDebug("Creating token for user {UserId} with {ClaimCount} claims", user.Id, authClaims.Count);

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
            var user = await _userManager.FindByEmailAsync(email) ?? throw new UnauthorizedAccessException("User not found");
            if (!await _userManager.VerifyUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation", otp))
                throw new Exception("Invalid or expired verification code");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);
            var changeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changeResult.Succeeded)
                throw new Exception(string.Join(", ", changeResult.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<UserInformationDTO> GetUserInfo(string email)
        {
            var user = await _userManager.Users
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

            return new UserInformationDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Gender = user.Gender,
                Address = address,
                PhoneNumber = phoneNumber
            };
        }

        public async Task UpdateUserInfo(UserInformationDTO userInfoDTO, string email)
        {
            try
            {
                // Get the user - simply using FindByEmailAsync to avoid the tracking issue
                var user = await _userManager.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

                Console.WriteLine($"Found user: ID={user.Id}, Email={user.Email}");

                // Copy only the fields we want to update
                user.FirstName = !string.IsNullOrWhiteSpace(userInfoDTO.FirstName) ? userInfoDTO.FirstName : user.FirstName;
                user.LastName = !string.IsNullOrWhiteSpace(userInfoDTO.LastName) ? userInfoDTO.LastName : user.LastName;
            user.Gender = userInfoDTO.Gender;
                user.PhoneNumber = !string.IsNullOrWhiteSpace(userInfoDTO.PhoneNumber) ? userInfoDTO.PhoneNumber : user.PhoneNumber;
                user.DisplayName = $"{user.FirstName} {user.LastName}".Trim();

                Console.WriteLine($"Updating user: FirstName={user.FirstName}, LastName={user.LastName}, Gender={user.Gender}, Phone={user.PhoneNumber}");

                // First, update the user information
                var result = await _userManager.UpdateAsync(user);
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
                var userWithAddress = await _userManager.Users
                    .AsNoTracking() // Important - don't track this entity
                    .Include(u => u.Address)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (userWithAddress == null)
                    throw new UserNotFoundException(email);

                Console.WriteLine($"Address update: User has address: {userWithAddress.Address != null}");

                // Get fresh User instance for update
                var user = await _userManager.FindByIdAsync(userWithAddress.Id);
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
                var result = await _userManager.UpdateAsync(user);
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
            var userWithAddress = await _userManager.Users
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
            
            var userRaw = await _userManager.Users
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

        private async Task CreateDoctorProfile(User user)
        {
            try
            {
                _logger.LogInformation("Creating doctor profile for user {UserId}", user.Id);
                
                // Create doctor profile using mapping profile
                var doctor = _mapper.Map<Doctor>(user);
                
                // Create or associate with clinic
                Guid clinicId;
                
                // Check if clinic info was provided in registration
                var clinicInfo = (user as User)?.ClinicInfo;
                if (clinicInfo != null)
                {
                    // Create new clinic with provided info
                    _logger.LogInformation("Creating new clinic for doctor {UserId}", user.Id);
                    var clinic = new Clinic
                    {
                        Name = clinicInfo.Name,
                        Address = clinicInfo.Address,
                        Phone = clinicInfo.Phone,
                        Email = clinicInfo.Email,
                        Description = clinicInfo.Description
                    };
                    
                    await _healthcareUnitOfWork.ClinicRepository.AddAsync(clinic);
                    await _healthcareUnitOfWork.SaveChangesAsync();
                    clinicId = clinic.Id;
                }
                else
                {
                    // Check if any clinic exists, if not create a default one
                    var clinics = await _healthcareUnitOfWork.ClinicRepository.GetAllAsync();
                    if (!clinics.Any())
                    {
                        _logger.LogInformation("No clinics found. Creating default clinic");
                        var defaultClinic = new Clinic
                        {
                            Name = "Default Clinic",
                            Address = "Please update address",
                            Phone = "Please update phone",
                            Email = "Please update email"
                        };
                        
                        await _healthcareUnitOfWork.ClinicRepository.AddAsync(defaultClinic);
                        await _healthcareUnitOfWork.SaveChangesAsync();
                        clinicId = defaultClinic.Id;
                    }
                    else
                    {
                        clinicId = clinics.First().Id;
                    }
                }
                
                // Associate doctor with clinic
                doctor.ClinicId = clinicId;
                
                await _healthcareUnitOfWork.DoctorRepository.AddAsync(doctor);
                await _healthcareUnitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Doctor profile created for user {UserId} with doctor ID {DoctorId}", 
                    user.Id, doctor.Id);
            }
            catch (Exception ex)
            {
                // Log the error but don't prevent user creation
                _logger.LogError(ex, "Error creating doctor profile for user {UserId}: {ErrorMessage}", 
                    user.Id, ex.Message);
                
                // We're intentionally not throwing here to allow user creation to succeed
                // even if profile creation fails
            }
        }
        
        private async Task CreatePetOwnerProfile(User user)
        {
            try
            {
                _logger.LogInformation("Creating pet owner profile for user {UserId}", user.Id);
                
                // Create pet owner profile using mapping profile
                var petOwner = _mapper.Map<PetOwner>(user);
                
                await _healthcareUnitOfWork.PetOwnerRepository.AddAsync(petOwner);
                await _healthcareUnitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Pet owner profile created for user {UserId} with pet owner ID {PetOwnerId}", 
                    user.Id, petOwner.Id);
            }
            catch (Exception ex)
            {
                // Log the error but don't prevent user creation
                _logger.LogError(ex, "Error creating pet owner profile for user {UserId}: {ErrorMessage}", 
                    user.Id, ex.Message);
                
                // We're intentionally not throwing here to allow user creation to succeed
                // even if profile creation fails
            }
        }
    }
}
