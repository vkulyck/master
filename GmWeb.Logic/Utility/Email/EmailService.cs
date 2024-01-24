using GmWeb.Logic.Utility.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Email;
public partial class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly IWebHostEnvironment _env;
    private readonly string _buildEnv;
    private readonly EmailClient _client;

    public EmailService(EmailSettings settings)
    {
        this._settings = settings;
        this._buildEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        this._client = new EmailClient(settings);
        this._settings.Validate();
    }
    public EmailService(IOptions<EmailSettings> options, IWebHostEnvironment env)
    {
        this._settings = options?.Value;
        this._env = env;
        this._buildEnv = env.EnvironmentName;
        this._client = new EmailClient(options, env);
        this._settings.Validate();
    }

    public async Task SendEmailConfirmationAsync(string EmailAddress, string CallbackUrl)
    {
        string Subject = "Confirm your email";
        string HtmlBody = $"Please confirm your email by clicking <a href='{CallbackUrl}'>here</a>.";

        await this.SendAsync(EmailAddress, Subject, HtmlBody).ConfigureAwait(false);
        }

    public async Task SendPasswordResetAsync(string EmailAddress, string CallbackUrl)
    {
        string Subject = "Reset your password";
        string HtmlBody = $"Please reset your password by clicking <a href='{CallbackUrl}'>here</a>.";
        await this.SendAsync(EmailAddress, Subject, HtmlBody).ConfigureAwait(false);
    }

    public async Task SendException(Exception ex)
    {
        string Subject = $"[{this._env.EnvironmentName}] INTERNAL SERVER ERROR";
        string HtmlBody = $"{ex.ToString()}";

        await this.SendAsync(this._settings.ErrorRecipient, Subject, HtmlBody).ConfigureAwait(false);
        }

        public async Task SendSqlException(SqlException ex)
        {
            string Subject = $"[{this._env.EnvironmentName}] SQL ERROR";
            string HtmlBody = $"{ex.ToString()}";

            await this.SendAsync(this._settings.ErrorRecipient, Subject, HtmlBody).ConfigureAwait(false);
        }

        public async Task SendAsync(string Recipients, string Subject, string HtmlBody)
        {
        if (!this._settings.Enabled)
                return;
        var html = new HtmlString(HtmlBody);
        await this._client.SendMessageAsync(Recipients, Subject, html).ConfigureAwait(false);
    }
}
