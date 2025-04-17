using Microsoft.AspNetCore.Http;

namespace Core.Services.Abstractions;

public interface IMailingService
{
    Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null);
}

