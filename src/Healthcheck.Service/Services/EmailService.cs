namespace Healthcheck.Service.Services
{
    using Healthcheck.Service.Interfaces;
    using Sitecore.Diagnostics;
    using System.Net;
    using System.Net.Mail;

    /// <summary>The class that handles email services.</summary>
    /// <seealso cref="Healthcheck.Service.Interfaces.IEmailService" />
    public class EmailService : IEmailService
    {
        /// <summary>The SMTP client</summary>
        private readonly SmtpClient smtpClient;

        /// <summary>Initializes a new instance of the <see cref="EmailService"/> class.</summary>
        public EmailService()
        {
            string serverName = Sitecore.Configuration.Settings.GetSetting("MailServer");
            int port = Sitecore.Configuration.Settings.GetIntSetting("MailServerPort", 25);
            string userName = Sitecore.Configuration.Settings.GetSetting("MailServerUserName");
            string password = Sitecore.Configuration.Settings.GetSetting("MailServerPassword");
            bool useSsl = Sitecore.Configuration.Settings.GetBoolSetting("MailServerUseSSL", false);

            Assert.IsNotNullOrEmpty(serverName, "Server name was null or empty");

            smtpClient = new SmtpClient
            {
                Port = port,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = useSsl,
                Host = serverName,
                Credentials = new NetworkCredential(userName, password)
            };
        }

        /// <summary>Sends the email.</summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public void SendEmail(string from, string to, string subject, string body)
        {
            Assert.IsNotNullOrEmpty(from, "From was null or empty");
            Assert.IsNotNullOrEmpty(to, "To was null or empty");

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(from);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                foreach (var address in to.Split(';'))
                {
                    if (!string.IsNullOrEmpty(address))
                    {
                        mail.To.Add(new MailAddress(address));
                    }
                }

                smtpClient.Send(mail);
            }
        }
    }
}