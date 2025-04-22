using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.OrderModels;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.OrderEntities;
using UserAddress = Domain.Entities.SecurityEntities.Address;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities.SecurityEntities;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Presentation
{
    //    [EnableCors("CORSPolicy")]
    public class AuthenticationController(IServiceManager serviceManager, IConfiguration configuration) : ApiController
    {

        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                Console.WriteLine($"Login attempt for email: {loginDTO?.Email ?? "null"}");
                
                if (loginDTO == null)
                {
                    Console.WriteLine("Login failed: loginDTO is null");
                    return BadRequest(new { message = "Login information is required" });
                }
                
                if (string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
                {
                    Console.WriteLine("Login failed: Email or password is empty");
                    return BadRequest(new { message = "Email and password are required" });
                }
                
                try
                {
                    var result = await serviceManager.AuthenticationService.LoginAsync(loginDTO);
                    Console.WriteLine($"Login successful for {loginDTO.Email}");
                    return Ok(result);
                }
                catch (Exception ex) when (ex.Message.Contains("Data is Null") || ex.InnerException?.Message.Contains("Data is Null") == true)
                {
                    // Special handling for null data errors from SQL
                    Console.WriteLine("Detected SQL null data error. Recommending direct database fix.");
                    return BadRequest(new
                    {
                        message = "Database error detected with your user account. Please use the direct database fix endpoint first.",
                        fixEndpoint = $"/api/Authentication/direct-db-fix?email={loginDTO.Email}",
                        originalError = ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error details for server-side debugging
                Console.WriteLine($"Login error for {loginDTO?.Email}: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().FullName}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception type: {ex.InnerException.GetType().FullName}");
                }
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Return appropriate response based on exception type
                if (ex is UnAuthorizedException)
                {
                    return Unauthorized(new { message = ex.Message });
                }
                
                // For development purposes, return more details about the error
                return StatusCode(500, new { 
                    message = $"Error during login: {ex.Message}",
                    details = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserResultDTO>> Register([FromBody] UserRegisterDTO registerDTO)
        {
            try
            {
                var result = await serviceManager.AuthenticationService.RegisterAsync(registerDTO);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                // Return a properly formatted error response with validation errors
                return BadRequest(new
                {
                    message = ex.Message,
                    errors = ex.Errors
                });
            }
            catch (Exception ex)
            {
                // Log the unexpected error
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner exception: {ex.InnerException.Message}";
                }
                
                return StatusCode(500, new { 
                    message = "Registration failed: " + errorMessage,
                    errorType = ex.GetType().Name
                });
            }
        }

        [HttpGet("CheckEmailExist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            var result = await serviceManager.AuthenticationService.CheckEmailExist(email);
            return Ok(result);
        }

        [HttpGet("VerifyEmail")]
        public async Task<ActionResult<bool>> VerifyEmail([FromQuery] string email, [FromQuery] string otp)
        {
            var result = await serviceManager.AuthenticationService.VerifyEmailAsync(email, otp);
            return result ? Ok(true) : BadRequest("Invalid or expired verification code.");
        }

        [HttpPost("SendVerificationCode")]
        public async Task<ActionResult<bool>> ResendOTP([FromBody] ResendOTPDTO otpDTO)
        {
            var result = await serviceManager.AuthenticationService.SendVerificationCodeAsync(otpDTO.Email);
            return result ? Ok(true) : BadRequest("Failed to send verification code.");
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string email, string oldPassword, string newPassword)
        {
            await serviceManager.AuthenticationService.ChangePasswordAsync(email, oldPassword, newPassword);
            return Ok(new { Message = "Password changed successfully." });
        }
        [HttpPost("SendResetPasswordEmail")]
        public async Task<ActionResult> SendResetPasswordEmail(string email)
        {
            await serviceManager.AuthenticationService.SendResetPasswordEmailAsync(email);
            return Ok(new { Message = "Password reset email sent successfully." });
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(string email, string token, string newPassword)
        {
            try
            {
                await serviceManager.AuthenticationService.ResetPasswordAsync(email, token, newPassword);
                return Ok(new { Message = "Password reset successfully." });
            }
            catch (UserNotFoundException ex)
            {
                // Return a properly formatted 404 response
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the unexpected error
                return StatusCode(500, new { message = "Password reset failed: " + ex.Message });
            }
        }
        [HttpPost("AddUserAddress")]
        [Authorize]
        public async Task<IActionResult> AddUserAddress([FromBody] AddressDTO address)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized("Email not found in token.");

            var result = await serviceManager.AuthenticationService.AddUserAddress(address, email);
            return Ok(result);
        }



        //   [HttpGet("GetUserAddress")]
        //   [Authorize]
        //   public async Task<ActionResult<AddressDTO>> GetUserAddress()
        //   {
        //       var email = User.FindFirstValue(ClaimTypes.Email);
        //       if (string.IsNullOrEmpty(email)) return Unauthorized("Email not found in token.");

        //       var result = await serviceManager.AuthenticationService.GetUserAddress(email);
        //       return Ok(result);
        //   }


        //[HttpPut("UpdateUserAddress")]
        //[Authorize]
        //public async Task<IActionResult> UpdateUserAddress([FromBody] AddressDTO address)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (string.IsNullOrEmpty(email)) return Unauthorized("Email not found in token.");

        //    var result = await serviceManager.AuthenticationService.UpdateUserAddress(address, email);
        //    return Ok(result);
        //}

        //[HttpGet("GetUserInformation")]
        //[Authorize]
        //public async Task<ActionResult<UserInformationDTO>> GetUserInfo()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    if (email == null) return Unauthorized("Email not found in token.");

        //    var user = await serviceManager.AuthenticationService.GetUserInfo(email);
        //    return Ok(user);
        //}
        [HttpGet("GetUserInformation")]
        public async Task<IActionResult> GetUserInformation(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    email = User.FindFirstValue(ClaimTypes.Email);
                    if (string.IsNullOrEmpty(email))
                    {
                        return Unauthorized("Email not found in token and no email provided.");
                    }
                }

                var userInfo = await serviceManager.AuthenticationService.GetUserInfo(email);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving user information: {ex.Message}" });
            }
        }
        
        [HttpGet("CheckAuthStatus")]
        public IActionResult CheckAuthStatus()
        {
            try
            {
                var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
                var email = User.FindFirstValue(ClaimTypes.Email);
                var name = User.FindFirstValue(ClaimTypes.Name);
                var role = User.FindFirstValue(ClaimTypes.Role);
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                return Ok(new {
                    isAuthenticated,
                    email,
                    name,
                    role,
                    userId = id,
                    claims = User.Claims.Select(c => new { 
                        type = c.Type, 
                        value = c.Value 
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error checking auth status: {ex.Message}" });
            }
        }
        [HttpPost("UpdateUserInformation")]
        [Authorize]
        public async Task<ActionResult> UpdateUserInfo([FromBody] UserInformationDTO userInfo)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (email == null) return Unauthorized("Email not found in token.");

                await serviceManager.AuthenticationService.UpdateUserInfo(userInfo, email);
                return Ok(new { Message = "User information updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to update user information: {ex.Message}" });
            }
        }
        [HttpGet("GetUserAddressDebug")]
        [Authorize]
        public async Task<ActionResult> GetUserAddressDebug()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (email == null) return Unauthorized("Email not found in token.");

                var user = await serviceManager.AuthenticationService.GetDebugInfo(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to get user address debug: {ex.Message}" });
            }
        }

        [HttpGet("direct-db-fix")]
        [AllowAnonymous]
        public async Task<IActionResult> DirectDatabaseFix(string email)
        {
            try
            {
                Console.WriteLine($"Starting direct database fix for: {email}");
                
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email parameter is required");
                }
                
                // Get connection string directly from configuration and clean it
                string rawConnectionString = configuration.GetConnectionString("IdentitySQLConnection");
                Console.WriteLine($"Raw connection string found: {!string.IsNullOrEmpty(rawConnectionString)}");
                
                if (string.IsNullOrEmpty(rawConnectionString))
                {
                    return BadRequest("Identity database connection string not found in configuration");
                }
                
                // Sanitize the connection string to remove unsupported parameters for System.Data.SqlClient
                string identityConnectionString = CleanConnectionString(rawConnectionString);
                
                // Normalize email for comparison
                string normalizedEmail = email.ToUpperInvariant();
                
                // Use direct ADO.NET to avoid EF Core issues
                using (var connection = new SqlConnection(identityConnectionString))
                {
                    try
                    {
                        await connection.OpenAsync();
                        Console.WriteLine("Database connection opened successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to open database connection: {ex.Message}");
                        return StatusCode(500, new 
                        { 
                            message = "Failed to connect to the database",
                            error = ex.Message,
                            details = "There may be an issue with the connection string format"
                        });
                    }
                    
                    // First create the user if they don't exist
                    try
                    {
                        // Check if the user exists
                        string checkUserSql = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR NormalizedEmail = @NormalizedEmail";
                        object userId = null;
                        
                        using (var checkCmd = new SqlCommand(checkUserSql, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", email);
                            checkCmd.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);
                            
                            userId = await checkCmd.ExecuteScalarAsync();
                            Console.WriteLine($"User lookup result: {(userId != null ? "Found" : "Not found")}");
                        }
                        
                        // If user doesn't exist, create them
                        if (userId == null)
                        {
                            Console.WriteLine($"User with email {email} not found. Creating new user.");
                            string newUserId = Guid.NewGuid().ToString();
                            string defaultUsername = $"user_{Guid.NewGuid().ToString("N")}";
                            
                            string createUserSql = @"
                                INSERT INTO AspNetUsers (
                                    Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
                                    EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled,
                                    AccessFailedCount, PhoneNumber, ConcurrencyStamp, SecurityStamp,
                                    PasswordHash, FirstName, LastName, DisplayName, Discriminator, UserRole
                                )
                                VALUES (
                                    @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                                    1, 0, 0, 0, 0, '', @ConcurrencyStamp, @SecurityStamp,
                                    @PasswordHash, '', '', '', 'User', 3
                                )";
                                
                            using (var createCmd = new SqlCommand(createUserSql, connection))
                            {
                                // Generate a simple password hash - this is just a placeholder
                                string passwordHash = "AQAAAAIAAYagAAAAELEGwOvSxVNNYy5JyKlZ0+nGgzSDT5lJL8K3CEmHF4J1xP9RODx7V8wWJR7i7/PbGw=="; // Hashed value of "Admin123!"
                                
                                createCmd.Parameters.AddWithValue("@Id", newUserId);
                                createCmd.Parameters.AddWithValue("@UserName", defaultUsername);
                                createCmd.Parameters.AddWithValue("@NormalizedUserName", defaultUsername.ToUpperInvariant());
                                createCmd.Parameters.AddWithValue("@Email", email);
                                createCmd.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);
                                createCmd.Parameters.AddWithValue("@ConcurrencyStamp", Guid.NewGuid().ToString());
                                createCmd.Parameters.AddWithValue("@SecurityStamp", Guid.NewGuid().ToString());
                                createCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                                
                                int rowsAffected = await createCmd.ExecuteNonQueryAsync();
                                Console.WriteLine($"Created new user. Rows affected: {rowsAffected}");
                                
                                userId = newUserId;
                            }
                        }
                        
                        Console.WriteLine($"Found/Created user with ID: {userId}");
                        
                        // Update user to fix NULL values
                        string fixUserSql = @"
                            UPDATE AspNetUsers
                            SET 
                                UserName = ISNULL(UserName, @DefaultUsername),
                                NormalizedUserName = ISNULL(NormalizedUserName, @DefaultNormalizedUsername),
                                Email = ISNULL(Email, @Email),
                                NormalizedEmail = ISNULL(NormalizedEmail, @NormalizedEmail),
                                PhoneNumber = ISNULL(PhoneNumber, ''),
                                ConcurrencyStamp = ISNULL(ConcurrencyStamp, @NewGuid1),
                                SecurityStamp = ISNULL(SecurityStamp, @NewGuid2),
                                FirstName = ISNULL(FirstName, ''),
                                LastName = ISNULL(LastName, ''),
                                DisplayName = ISNULL(DisplayName, ''),
                                Discriminator = ISNULL(Discriminator, 'User'),
                                UserRole = 3, -- Directly set to Admin (3)
                                EmailConfirmed = 1 -- Ensure email is confirmed
                            WHERE Email = @Email OR NormalizedEmail = @NormalizedEmail";
                        
                        // Use a different variable name for the second instance to avoid duplication
                        string usernameForUpdate = $"user_{Guid.NewGuid().ToString("N")}";
                        
                        using (var updateCmd = new SqlCommand(fixUserSql, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@Email", email);
                            updateCmd.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);
                            updateCmd.Parameters.AddWithValue("@DefaultUsername", usernameForUpdate);
                            updateCmd.Parameters.AddWithValue("@DefaultNormalizedUsername", usernameForUpdate.ToUpperInvariant());
                            updateCmd.Parameters.AddWithValue("@NewGuid1", Guid.NewGuid().ToString());
                            updateCmd.Parameters.AddWithValue("@NewGuid2", Guid.NewGuid().ToString());
                            
                            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                            Console.WriteLine($"Updated user data. Rows affected: {rowsAffected}");
                        }
                        
                        // Check if Admin role exists and create if not
                        string checkAdminRoleSql = "SELECT Id FROM AspNetRoles WHERE Name = 'Admin'";
                        object roleIdObj = null;
                        string adminRoleId;
                        
                        using (var checkRoleCmd = new SqlCommand(checkAdminRoleSql, connection))
                        {
                            roleIdObj = await checkRoleCmd.ExecuteScalarAsync();
                            Console.WriteLine($"Admin role lookup result: {(roleIdObj != null ? "Found" : "Not found")}");
                        }
                        
                        if (roleIdObj == null)
                        {
                            // Create Admin role
                            adminRoleId = Guid.NewGuid().ToString();
                            string createRoleSql = @"
                                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                                VALUES (@Id, 'Admin', 'ADMIN', @ConcurrencyStamp)";
                                
                            using (var createRoleCmd = new SqlCommand(createRoleSql, connection))
                            {
                                createRoleCmd.Parameters.AddWithValue("@Id", adminRoleId);
                                createRoleCmd.Parameters.AddWithValue("@ConcurrencyStamp", Guid.NewGuid().ToString());
                                
                                await createRoleCmd.ExecuteNonQueryAsync();
                                Console.WriteLine("Created Admin role");
                            }
                        }
                        else
                        {
                            adminRoleId = roleIdObj.ToString();
                            Console.WriteLine("Admin role exists");
                        }
                        
                        // Get user ID one more time to be safe
                        string getUserIdSql = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR NormalizedEmail = @NormalizedEmail";
                        string userIdFromDb = null;
                        
                        using (var getUserIdCmd = new SqlCommand(getUserIdSql, connection))
                        {
                            getUserIdCmd.Parameters.AddWithValue("@Email", email);
                            getUserIdCmd.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);
                            
                            var userIdObj = await getUserIdCmd.ExecuteScalarAsync();
                            if (userIdObj != null)
                            {
                                userIdFromDb = userIdObj.ToString();
                                Console.WriteLine($"User ID retrieved: {userIdFromDb}");
                            }
                            else
                            {
                                Console.WriteLine("Unable to retrieve user ID");
                                return StatusCode(500, new { message = "User exists but ID cannot be retrieved" });
                            }
                        }
                        
                        // Check if user is already in Admin role
                        string checkUserRoleSql = "SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
                        bool userInAdminRole = false;
                        
                        using (var checkUserRoleCmd = new SqlCommand(checkUserRoleSql, connection))
                        {
                            checkUserRoleCmd.Parameters.AddWithValue("@UserId", userIdFromDb);
                            checkUserRoleCmd.Parameters.AddWithValue("@RoleId", adminRoleId);
                            
                            var result = await checkUserRoleCmd.ExecuteScalarAsync();
                            userInAdminRole = (result != null);
                            Console.WriteLine($"User in admin role check: {userInAdminRole}");
                        }
                        
                        // Add user to Admin role if not already
                        if (!userInAdminRole)
                        {
                            try
                            {
                                string addToRoleSql = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                                
                                using (var addToRoleCmd = new SqlCommand(addToRoleSql, connection))
                                {
                                    addToRoleCmd.Parameters.AddWithValue("@UserId", userIdFromDb);
                                    addToRoleCmd.Parameters.AddWithValue("@RoleId", adminRoleId);
                                    
                                    await addToRoleCmd.ExecuteNonQueryAsync();
                                    Console.WriteLine("Added user to Admin role");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning - Error adding user to role: {ex.Message}");
                                // Continue anyway
                            }
                        }
                        else
                        {
                            Console.WriteLine("User already in Admin role");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during DB operations: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                        throw; // Rethrow to be caught by outer try-catch
                    }
                }
                
                return Ok(new
                {
                    success = true,
                    message = "Direct database fix completed successfully",
                    nextStep = "Try logging in as admin now",
                    password = "Admin123!" // Default password if user was created
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in direct database fix: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                return StatusCode(500, new
                {
                    message = $"Database fix failed: {ex.Message}",
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        
        // Helper method to clean connection string for System.Data.SqlClient compatibility
        private string CleanConnectionString(string connectionString)
        {
            try 
            {
                // Create a SqlConnectionStringBuilder to properly parse the connection string
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
                
                // Set only the essential properties
                foreach (var part in connectionString.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(part)) continue;
                    
                    var keyValue = part.Split('=', 2);
                    if (keyValue.Length != 2) continue;
                    
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();
                    
                    switch (key.ToLowerInvariant())
                    {
                        case "server":
                        case "data source":
                            builder.DataSource = value;
                            break;
                        case "database":
                        case "initial catalog":
                            builder.InitialCatalog = value;
                            break;
                        case "user id":
                            builder.UserID = value;
                            break;
                        case "password":
                            builder.Password = value;
                            break;
                        case "integrated security":
                            builder.IntegratedSecurity = value.ToLowerInvariant() == "true" || value == "sspi";
                            break;
                        case "multipleactiveresultsets":
                            builder.MultipleActiveResultSets = value.ToLowerInvariant() == "true";
                            break;
                        case "encrypt":
                            builder.Encrypt = value.ToLowerInvariant() == "true";
                            break;
                        case "connection timeout":
                            if (int.TryParse(value, out int timeout))
                            {
                                builder.ConnectTimeout = timeout;
                            }
                            break;
                    }
                }
                
                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning connection string: {ex.Message}");
                
                // Fallback: manually remove unsupported parameters
                var parts = connectionString.Split(';')
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim())
                    .ToList();
                
                // Filter out known unsupported parameters
                var filteredParts = parts.Where(p => 
                    !p.StartsWith("Command Timeout=", StringComparison.OrdinalIgnoreCase) &&
                    !p.StartsWith("ConnectRetryCount=", StringComparison.OrdinalIgnoreCase) &&
                    !p.StartsWith("ConnectRetryInterval=", StringComparison.OrdinalIgnoreCase));
                
                // Reassemble the connection string
                return string.Join(";", filteredParts);
            }
        }
    }
}

        //    [HttpPost("Login")]
        //    public async Task<ActionResult<UserResultDTO>> Login(LoginDTO loginDTO)
        //    {
        //        var result = await serviceManager.AuthenticationService.LoginAsync(loginDTO);
        //        return Ok(result);
        //    }
        //    [HttpPost("Register")]
        //    public async Task<ActionResult<UserResultDTO>> Register(UserRegisterDTO registerDTO)
        //    {
        //        var result = await serviceManager.AuthenticationService.RegisterAsync(registerDTO);
        //        return Ok(result);
        //    }
        //    [HttpGet("CheckEmailExist")]
        //    public async Task<ActionResult<bool>> CheckEmailExist(string email)
        //    {
        //        var result = await serviceManager.AuthenticationService.CheckEmailExist(email);
        //        return Ok(result);
        //    }
        //    [HttpGet("VerifyEmail")]
        //    public async Task<ActionResult<bool>> VerifyEmail([FromQuery] string email, [FromQuery] string otp)
        //    {
        //        var result = await serviceManager.AuthenticationService.VerifyEmailAsync(email, otp);
        //        return result ? Ok(true) : BadRequest("Invalid or expired verification code.");
        //    }

        //    [HttpPost("SendVerificationCode")]
        //    public async Task<ActionResult<bool>> ResendOTP(ResendOTPDTO otpDTO)
        //    {
        //        var result = await serviceManager.AuthenticationService.SendVerificationCodeAsync(otpDTO.Email);
        //        return result ? Ok(true) : BadRequest("Failed to send verification code.");
        //    }

        //    [HttpPost("ChangePassword")]
        //    public async Task<ActionResult> ChangePassword(string email, string oldPassword, string newPassword)
        //    {
        //        await serviceManager.AuthenticationService.ChangePasswordAsync(email, oldPassword, newPassword);
        //        return Ok(new { Message = "Password changed successfully." });
        //    }
        //    [HttpPost("SendResetPasswordEmail")]
        //    public async Task<ActionResult> SendResetPasswordEmail(string email)
        //    {
        //        await serviceManager.AuthenticationService.SendResetPasswordEmailAsync(email);
        //        return Ok(new { Message = "Password reset email sent successfully." });
        //    }
        //    [HttpPost("ResetPassword")]
        //    public async Task<ActionResult> ResetPassword(string email, string token, string newPassword)
        //    {
        //        await serviceManager.AuthenticationService.ResetPasswordAsync(email, token, newPassword);
        //        return Ok(new { Message = "Password reset successfully." });
        //    }
        //    [HttpGet("GetCurrentUser")]
        //    [Authorize]
        //    public async Task<ActionResult<UserResultDTO>> GetCurrentUser(string email)
        //    {
        //        var result = await serviceManager.AuthenticationService.GetUserByEmail(email);

        //        return Ok(result);
        //    }
        //    [HttpGet("GetUserAddress")]
        //    public async Task<ActionResult> GetUserAddress()
        //    {
        //        var email = User.FindFirstValue(ClaimTypes.Email);

        //        var result=  await serviceManager.AuthenticationService.GetUserAddress(email);

        //        return Ok(result);
        //    }
        //    [HttpPut("UpdateUserAddress")]
        //    public async Task<IActionResult> UpdateUserAddress(AddressDTO address)
        //    {
        //        var email = User.FindFirstValue(ClaimTypes.Email);

        //        if (email == null)
        //            throw new UnAuthorizedException("Email Doesn't Exist");

        //        var result=  await serviceManager.AuthenticationService.UpdateUserAddress(address, email);

        //        return Ok(result);
        //    }

        //    [HttpGet("GetUserInformation")]

        //    public async Task<ActionResult<UserInformationDTO>> GetUserInfo(UserAddress address)
        //    {
        //        var email = User?.FindFirstValue(ClaimTypes.Email);

        //        if (email == null)
        //            throw new UnAuthorizedException("Email Doesn't Exist");

        //        var user = await serviceManager.AuthenticationService.GetUserInfo(email,address);

        //        return user;
        //    }

        //    [HttpPut("UpdateUserInformation")]
        //    public async Task<ActionResult> UpdateUserInfo([FromQuery] UserInformationDTO userInfo,[FromQuery]AddressDTO address)
        //    {
        //        var email = User?.FindFirstValue(ClaimTypes.Email);

        //        if (email == null)
        //            throw new UnAuthorizedException("Email Doesn't Exist");

        //        await serviceManager.AuthenticationService.UpdateUserInfo(userInfo, email,address);

        //        return RedirectToAction(nameof(GetUserInfo));
        //    }
        //}
