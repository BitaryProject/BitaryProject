using Microsoft.AspNetCore.Mvc;
using Core.Services.Abstractions;
using Shared.MailModels;

namespace BitaryProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailingController(IMailingService _mailingService) :ControllerBase
    {

        [HttpPost("Send")]
        //FromForm 3shan el data ely gaya mn el user haykon feha attachment fa msh haynf3 tkon mn l body
        public async Task<ActionResult> SendMail([FromForm] MailRequestDTO dTO)
        {
            await _mailingService.SendEmailAsync(dTO.ToEmail,dTO.Subject,dTO.Body,dTO.Attachments);
            return Ok();
        }
    }
}
