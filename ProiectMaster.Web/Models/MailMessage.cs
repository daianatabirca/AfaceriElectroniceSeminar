using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ProiectMaster.Web.Models
{
    public class MailMessage
    {
        public List<string> To { get; set; }

        public string Subject { get; set; }

        public IFormFile File { get; set; }

        public List<IFormFile> Files { get; set; }
    }
}
