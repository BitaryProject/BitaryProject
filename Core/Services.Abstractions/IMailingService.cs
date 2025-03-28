﻿using Microsoft.AspNetCore.Http;

namespace Services.Abstractions;

public interface IMailingService
{
    Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);
}
