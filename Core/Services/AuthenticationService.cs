using Domain.Entities.SecurityEntities;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.OrderModels;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserAddress = Domain.Entities.SecurityEntities.Address;


namespace Services
{
    public class AuthenticationService(
        UserManager<User> userManager,
        IOptions<JwtOptions> options,
        IOptions<DomainSettings> domainOptions
        ,IMapper mapper,
        IMailingService mailingService) : IAuthenticationService
    {
        public async Task<bool> CheckEmailExist(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            return user != null;
        }

        public async Task<AddressDTO> GetUserAddress(string email)
        {
            var user =await userManager.Users
                .Include(u=>u.Address).FirstOrDefaultAsync(u=>u.Email ==email)
                ?? throw new UserNotFoundException(email);

            return mapper.Map<AddressDTO>(user.Address);
            //return new AddressDTO
            //{
            //    City=user.Address.City,
            //    Country=user.Address.Country,
            //    FirstName=user.Address.FirstName,
            //    LastName=user.Address.LastName
            //};
        }

        public async Task<UserResultDTO> GetUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email)??throw new UserNotFoundException(email);

            return new UserResultDTO(user.DisplayName,
                         user.Email,
                          await CreateTokenAsync(user));
        }

        public async Task<UserResultDTO> LoginAsync(LoginDTO loginModel)
        {
            var user= await userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
                throw new UnAuthorizedException("Email Doesn't Exist");
            var result = await userManager.CheckPasswordAsync(user, loginModel.Password);
            if(!result)
                throw new UnAuthorizedException(); 

            return new UserResultDTO(user.DisplayName,
                user.Email,
                 await CreateTokenAsync(user));

        }

        //public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
        //{
        //    var user = new User
        //    {

        //        DisplayName = registerModel.DisplayName,
        //        Email = registerModel.Email,
        //        PhoneNumber = registerModel.PhoneNumber,
        //        UserName = registerModel.UserName,
        //    };

        //    var result = await userManager.CreateAsync(user, registerModel.Password);
        //    if (!result.Succeeded)
        //    {
        //        var errors=result.Errors.Select(e=>e.Description).ToList();
        //        throw new ValidationException(errors);
        //    }
        //    return new UserResultDTO(
        //                user.DisplayName,
        //                 user.Email,
        //               await  CreateTokenAsync(user));

        
        //}

        public async Task<AddressDTO> UpdateUserAddress(AddressDTO address,string email)
        {
            var user = await userManager.Users
                           .Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email)
                           ?? throw new UserNotFoundException(email);


            if(user.Address != null)
            {
                user.Address.FirstName = address.FirstName;
                user.Address.LastName = address.LastName;
                user.Address.Street = address.Street;
                user.Address.City = address.City;
                user.Address.Country = address.Country;
            }
            else
            {
                var userAddress=mapper.Map<UserAddress>(address);
                user.Address = userAddress;
            }

            await userManager.UpdateAsync(user);

            return address;
        }

        private async Task<string> CreateTokenAsync(User user)
        {
            var jwtOptions = options.Value;

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };

            //Add role to this user if he has exist role
            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

            var signingCreds=new SigningCredentials(key ,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: jwtOptions.Audience,
                issuer: jwtOptions.Issuer,
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                claims:authClaims,
                signingCredentials:signingCreds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
        {
            if (await userManager.FindByEmailAsync(registerModel.Email) != null)
                throw new ValidationException(new List<string> { "Email is already taken." });

            if (await userManager.FindByNameAsync(registerModel.UserName) != null)
                throw new ValidationException(new List<string> { "Username is already taken." });

            var user = new User()
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                DisplayName = $"{registerModel.FirstName} {registerModel.LastName}",
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
                UserName = registerModel.UserName,
            };
            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }
            var DomainOptions = domainOptions.Value;
            var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");

            await mailingService.SendEmailAsync(user.Email!, "Verification Code", $"{DomainOptions.localUrl}api/Authentication/VerifyEmail?email={registerModel.Email}&otp={verificationCode}");



            return new UserResultDTO(
               user.DisplayName,
               user.Email!,
               await CreateTokenAsync(user));
        }

        public async Task<bool> VerifyEmailAsync(string email, string otp)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new UnauthorizedAccessException("User not found");

            var isValid = await userManager.VerifyUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation", otp);

            //var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "EmailConfirmation");


            if (!isValid) throw new Exception("Invalid or expired verification code");

            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);

            return true;
        }


        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new UnauthorizedAccessException("User not found");

            var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "email_confirmation");

            //var verificationCode = await userManager.GenerateUserTokenAsync(user, "CustomEmailTokenProvider", "EmailConfirmation");

            var DomainOptions = domainOptions.Value;

            await mailingService.SendEmailAsync(user.Email!, "Verification Code", $"{DomainOptions.localUrl}api/Authentication/VerifyEmail?email={email}&otp={verificationCode}");

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new UserNotFoundException(email);

            var changeResult = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changeResult.Succeeded)
                throw new Exception(string.Join(", ", changeResult.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<bool> SendResetPasswordEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            // Update the security stamp to invalidate any previous tokens
            await userManager.UpdateSecurityStampAsync(user);

            // Generate a new password reset token after updating the security stamp
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var DomainOptions = domainOptions.Value;

            var resetLink = $"{DomainOptions.localUrl}api/Authentication/ResetPassword?email={email}&token={token}";

            // Send the reset password email with the generated link
            await mailingService.SendEmailAsync(user.Email!, "Reset Password", $"Click the link to reset your password: {resetLink}");

            return true;
        }
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new UserNotFoundException(email);

            var isValidToken = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);
            if (!isValidToken) throw new Exception("Invalid or expired token");

            var resetResult = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!resetResult.Succeeded)
                throw new Exception(string.Join(", ", resetResult.Errors.Select(e => e.Description)));

            return true;
        }
            
    
    
    
    }


}
