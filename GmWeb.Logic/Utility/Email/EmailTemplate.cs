using Microsoft.AspNetCore.Html;
using System.IO;
using System.Xml;

namespace GmWeb.Logic.Utility.Email
{
    public class EmailTemplate
    {
        public string Name { get; private set; }
        public string Subject { get; private set; }
        public string BodyTemplate { get; private set; }
        public HtmlString Body { get; private set; }
        public object[] Arguments { get; private set; }

        public EmailTemplate(string name, params object[] args)
        {
            this.Name = name;
            this.Arguments = args;
            this.ProcessTemplate(name, args);
        }

        protected void ProcessTemplate(string name, params object[] args)
        {
            using (var stream = typeof(EmailClient).Assembly.GetManifestResourceStream($"GmWeb.Logic.Utility.Email.Templates.{name}"))
            {
                var reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                var doc = new XmlDocument();
                doc.LoadXml(content);

                this.Subject = doc.SelectSingleNode("email/subject").InnerText;
                this.BodyTemplate = doc.SelectSingleNode("email/body").InnerXml;
                this.Body = new HtmlString(SmartFormat.Smart.Format(this.BodyTemplate, args));
            }
        }
    }
}
