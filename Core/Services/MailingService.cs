using BitaryProject.Mail;
using  MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;

public class MailingService : IMailingService
{
    private readonly MailSettings _mailSettings;

    public MailingService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }
    public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null)
    {
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Email),
            Subject=subject
        };
        email.To.Add(MailboxAddress.Parse(mailTo));

        var builder = new BodyBuilder();// Beykon feh attachments w el body bta3 l email nafso
        if(attachments != null)//check lw feh attachments mab3ota ba7ota fl body
        {
            byte[] fileBytes;
            foreach(var file in attachments)
            {
                if(file.Length > 0)
                {
                    using var ms = new MemoryStream();//read attachment file
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody= body;
        email.Body=builder.ToMessageBody();

        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

        using var smtp = new SmtpClient();

        smtp.Connect(_mailSettings.Host,_mailSettings.Port,SecureSocketOptions.StartTls);

        smtp.Authenticate(_mailSettings.Email,_mailSettings.Password);

        await smtp.SendAsync(email);

        smtp.Disconnect(true);
    }
}
