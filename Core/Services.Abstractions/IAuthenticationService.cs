using Shared.OrderModels;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAddress = Domain.Entities.SecurityEntities.Address;


namespace Services.Abstractions
{
    public interface IAuthenticationService
    {
        public Task<UserResultDTO> LoginAsync(LoginDTO loginModel);
        public Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel);

        public Task<UserResultDTO> GetUserByEmail(string email);

        public Task<bool> CheckEmailExist(string email);

        public Task<AddressDTO> GetUserAddress(string email);

        public Task<AddressDTO> UpdateUserAddress(AddressDTO address,string email);
        // OTP Verification
        public Task<bool> VerifyEmailAsync(string email, string otp);

        // Resend OTP
        public Task<bool> SendVerificationCodeAsync(string email);
        // Send Reset Password Email
        public Task<bool> SendResetPasswordEmailAsync(string email);

        // Change Password
        public Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);

        // Reset Password
        public Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        public Task<UserInformationDTO> GetUserInfo(string email, UserAddress userAddress);
        public Task UpdateUserInfo(UserInformationDTO userInfoDTO, string email, AddressDTO address);



    }
}
