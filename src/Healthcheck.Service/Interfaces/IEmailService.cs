namespace Healthcheck.Service.Interfaces
{
    /// <summary>
    /// The interface for email services.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>Sends the email.</summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        void SendEmail(string from, string to, string subject, string body);
    }
}