using GmWeb.Logic.Utility.Extensions.Html;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Email
{
    public class EmailClient : IDisposable
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IHostEnvironment _env;
        private readonly EmailSettings _settings;
        private readonly string _buildEnv;

        protected IHostEnvironment HostEnvironment => this._env;
        protected EmailSettings Settings => this._settings;
        protected string BuildEnvironment => this._buildEnv;
        public EmailClient(EmailSettings settings)
        {
            this._settings = settings;
            this._buildEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
        public EmailClient(IOptions<EmailSettings> options, IHostEnvironment env)
        {
            this._settings = options?.Value;
            this._env = env;
            this._buildEnv = env.EnvironmentName;
        }

        public static Dictionary<string, string> TemplateSubjects { get; } = new Dictionary<string, string>
        {
            {"PasswordResetTemplate.htm", "Password Reset Token" }
        };

        public async Task<bool> SendPasswordResetTokenAsync(string mailto, string token, string url)
        {
            var template = new EmailTemplate("PasswordResetTemplate.htm", new { token, url });
            return await this.SendMessageAsync(mailto, template.Subject, template.Body);
        }
        public async Task<bool> SendRegistrationTokenAsync(string email, string token, string url)
        {
            var template = new EmailTemplate("RegistrationTemplate.htm", new { email, token, url });
            return await this.SendMessageAsync(email, template.Subject, template.Body);
        }

        public async Task<bool> SendPlainMessageAsync(string recipients, string subject, string body)
            => await this.SendMessageAsync(recipients, subject, body: body, isBodyHtml: false);
        public async Task<bool> SendHtmlMessageAsync(string recipients, string subject, string body)
            => await this.SendMessageAsync(recipients, subject, body: body, isBodyHtml: true);
        public async Task<bool> SendMessageAsync(string recipients, string subject, IHtmlContent body)
        {
            string html = body.ToHtmlString();
            return await this.SendMessageAsync(recipients, subject, body: html, isBodyHtml: true);
        }
        protected virtual async Task<bool> SendMessageAsync(string recipients, string subject, string body, bool isBodyHtml)
        {
            var mail = new MailMessage();
            using (var client = new SmtpClient(this.Settings.SmtpServer, this.Settings.SmtpPort.Value))
            {

                mail.From = new MailAddress(this.Settings.DefaultSender);
                if (string.IsNullOrEmpty(this.Settings.OverrideRecipient))
                    mail.To.Add(recipients);
                else
                    mail.To.Add(this.Settings.OverrideRecipient);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = isBodyHtml;

                client.EnableSsl = this.Settings.EnableSsl;
                client.Credentials = new System.Net.NetworkCredential(this.Settings.SmtpLogin, this.Settings.SmtpPassword);

                try
                {
                    _logger.Info("Sending email...");
                    await client.SendMailAsync(mail);
                    _logger.Info("Complete.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return false;
                }
                return true;
            }
        }

        public virtual void Dispose() { }
    }
}
