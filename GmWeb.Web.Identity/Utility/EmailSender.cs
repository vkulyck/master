using GmWeb.Logic.Utility.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Utility.Email
{
    public class EmailSender : EmailClient, IEmailSender
    {
        public EmailSender(EmailSettings settings) : base(settings) { }
        public EmailSender(IOptions<EmailSettings> options, IHostEnvironment env) : base(options, env) { }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage) => await this.SendHtmlMessageAsync(email, subject, htmlMessage);
    }
}
