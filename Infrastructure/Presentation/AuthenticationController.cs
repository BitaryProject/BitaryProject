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


namespace Presentation
{
    //    [EnableCors("CORSPolicy")]
    public class AuthenticationController(IServiceManager serviceManager) : ApiController
    {

        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await serviceManager.AuthenticationService.LoginAsync(loginDTO);
            return Ok(result);
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
