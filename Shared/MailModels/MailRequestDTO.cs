using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MailModels
{
    public class MailRequestDTO
    {
        [Required]
        public string ToEmail { get; set; }
        [Required]

        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public IList<IFormFile>? Attachments { get; set; }
    }
}
