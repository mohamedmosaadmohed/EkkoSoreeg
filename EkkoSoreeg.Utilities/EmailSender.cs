using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EkkoSoreeg.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _ssl;
        private readonly bool _isBodyHtml;

        public EmailSender(string email, string password, string host, bool ssl, int port, bool isBodyHtml)
        {
            _email = email;
            _password = password;
            _host = host;
            _ssl = ssl;
            _port = port;
            _isBodyHtml = isBodyHtml;
        }

        public Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                var message = new MailMessage(_email, to)
                {
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = _isBodyHtml
                };

                var credentials = new NetworkCredential(_email, _password);
                var smtpClient = new SmtpClient(_host)
                {
                    EnableSsl = _ssl,
                    Port = _port,
                    Credentials = credentials
                };

                smtpClient.Send(message);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email", ex);
            }
        }
    }
}
