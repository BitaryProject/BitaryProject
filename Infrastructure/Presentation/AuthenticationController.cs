using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
