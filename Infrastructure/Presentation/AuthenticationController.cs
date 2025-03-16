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

namespace Presentation
{
    public class AuthenticationController(IServiceManager serviceManager):ApiController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDTO>> Login(LoginDTO loginDTO)
        {
            var result=await serviceManager.AuthenticationService.LoginAsync(loginDTO);

            return Ok(result);
        } 
        [HttpPost("Register")]
        public async Task<ActionResult<UserResultDTO>> Register(UserRegisterDTO registerDTO)
        {
            var result=await serviceManager.AuthenticationService.RegisterAsync(registerDTO);

            return Ok(result);
        }
        [HttpGet("Check Email Exist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            return Ok(await serviceManager.AuthenticationService.CheckEmailExist(email));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserResultDTO>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var result = await serviceManager.AuthenticationService.GetUserByEmail(email);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var result = await serviceManager.AuthenticationService.GetUserAddress(email);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("Address")] // HttpPut => Like HttpPost but it is used for update
        public async Task<ActionResult<AddressDTO>> UpdateAddress(AddressDTO address)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var result = await serviceManager.AuthenticationService.UpdateUserAddress(address, email);

            return Ok(result);
        }
    }
}
