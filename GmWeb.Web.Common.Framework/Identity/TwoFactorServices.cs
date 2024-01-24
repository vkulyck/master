using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Threading.Tasks;
using SmsSettings = GmWeb.Logic.Utility.Phone.SmsSettings;
using EmailSettings = GmWeb.Logic.Utility.Email.EmailSettings;
using EmailClient = GmWeb.Logic.Utility.Email.EmailClient;

namespace GmWeb.Web.Common.Identity
{
    public class EmailService : IIdentityMessageService, IDisposable
    {
        public EmailClient Client { get; private set; }
        public EmailService(EmailSettings settings)
        {
            this.Client = new EmailClient(settings);
        }
        public async Task SendAsync(IdentityMessage message)
        {
            await this.Client.SendHtmlMessageAsync(message.Destination, message.Subject, message.Body);
        }
        public void Dispose() { }
    }
    public class SmsService : IIdentityMessageService, IDisposable
    {
        public SmsSettings Settings { get; private set; }
        public SmsService(SmsSettings settings)
        {
            this.Settings = settings;
        }
        public async Task SendAsync(IdentityMessage message)
        {
            var sms = new GmWeb.Logic.Utility.Phone.Message
            (
                senderId: this.Settings.SenderID,
                phoneNumber: message.Destination,
                textMessage: message.Body,
                type: GmWeb.Logic.Utility.Phone.MessageType.Transactional
            );
            await GmWeb.Logic.Utility.Phone.SMSService.SendAsync(sms);
        }

        public void Dispose() { }
    }
}