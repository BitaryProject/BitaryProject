using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class AuthenticationController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDTO>> Login(LoginDTO loginDTO)
        {
            var result = await serviceManager.AuthenticationService.LoginAsync(loginDTO);
            return Ok(result);
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserResultDTO>> Register(UserRegisterDTO registerDTO)
        {
            var result = await serviceManager.AuthenticationService.RegisterAsync(registerDTO);
            return Ok(result);
        }
        [HttpGet("CheckEmailExist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            var result = await serviceManager.AuthenticationService.CheckEmailExist(email);
            return Ok(result);
        }
        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserResultDTO>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await serviceManager.AuthenticationService.GetUserByEmail(email);
            return Ok(result);
        }
        [HttpGet("VerifyEmail")]
        public async Task<ActionResult<bool>> VerifyEmail([FromQuery] string email, [FromQuery] string otp)
        {
            var result = await serviceManager.AuthenticationService.VerifyEmailAsync(email, otp);
            return result ? Ok(true) : BadRequest("Invalid or expired verification code.");
        }

        [HttpPost("SendVerificationCode")]
        public async Task<ActionResult<bool>> ResendOTP(ResendOTPDTO otpDTO)
        {
            var result = await serviceManager.AuthenticationService.SendVerificationCodeAsync(otpDTO.Email);
            return result ? Ok(true) : BadRequest("Failed to send verification code.");
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string email, string oldPassword, string newPassword)
        {
            await serviceManager.AuthenticationService.ChangePasswordAsync(email, oldPassword, newPassword);
            return Ok(new { Message = "Password changed successfully." });
        }
        [HttpPost("SendResetPasswordEmail")]
        public async Task<IActionResult> SendResetPasswordEmail(string email)
        {
            await serviceManager.AuthenticationService.SendResetPasswordEmailAsync(email);
            return Ok(new { Message = "Password reset email sent successfully." });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            await serviceManager.AuthenticationService.ResetPasswordAsync(email, token, newPassword);
            return Ok(new { Message = "Password reset successfully." });
        }
    }
}
