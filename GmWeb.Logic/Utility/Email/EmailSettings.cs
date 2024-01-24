using System;

namespace GmWeb.Logic.Utility.Email
{
    public class EmailSettings
    {
        public bool Enabled { get; set; }

        public string SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public string SmtpLogin { get; set; }
        public string SmtpPassword { get; set; }
        public bool EnableSsl { get; set; }

        public string DefaultSender { get; set; }
        public string OverrideRecipient { get; set; }
        public string ErrorRecipient { get; set; }

        public void Validate()
        {
            if (!this.Enabled)
                return;
            if (string.IsNullOrEmpty(this.SmtpServer))
                throw new ArgumentException($"SMTP server missing from email settings.");
            if (!this.SmtpPort.HasValue || this.SmtpPort.Value <= 0 || this.SmtpPort.Value >= 1 << 16)
                throw new ArgumentException($"SMTP port missing from email settings.");
            if (string.IsNullOrEmpty(this.SmtpLogin))
                throw new ArgumentException($"SMTP login missing from email settings.");
            if (string.IsNullOrEmpty(this.SmtpPassword))
                throw new ArgumentException($"SMTP password missing from email settings.");
            if (string.IsNullOrEmpty(this.DefaultSender))
                throw new ArgumentException($"Default sender missing from email settings.");
        }

    }
}
