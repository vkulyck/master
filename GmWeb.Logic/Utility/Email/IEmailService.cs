using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Email;
public partial interface IEmailService
{
    Task SendAsync(string Recipients, string Subject, string HtmlBody);
    Task SendEmailConfirmationAsync(string Recipients, string CallbackUrl);
    Task SendPasswordResetAsync(string Recipients, string CallbackUrl);
    Task SendException(Exception ex);
    Task SendSqlException(SqlException ex);
}
