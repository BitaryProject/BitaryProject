using Shared.OrderModels;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAddress = Core.Domain.Entities.SecurityEntities.Address;


namespace Core.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<UserResultDTO> LoginAsync(LoginDTO loginModel);
        Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel);
        Task<UserResultDTO> GetUserByEmail(string email);
        Task<bool> CheckEmailExist(string email);
        Task<AddressDTO> GetUserAddress(string email);
        Task<AddressDTO> UpdateUserAddress(AddressDTO address, string email);

        // OTP Verification
        Task<bool> VerifyEmailAsync(string email, string otp);
        Task<bool> SendVerificationCodeAsync(string email);

        // Password Management
        Task<bool> SendResetPasswordEmailAsync(string email);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        // User Information
        Task<UserInformationDTO> GetUserInfo(string email);


        Task<AddressDTO> AddUserAddress(AddressDTO address, string email);
        Task UpdateUserInfo(UserInformationDTO userInfoDTO, string email);

        // Debug helper
        Task<object> GetDebugInfo(string email);
    }
}

