using System.Net;
using System.Net.Mail;

namespace VakaCalismasi
{
    public interface IEmailService
    {
        void SendEmail(string recipient, string subject, string body);
    }

    public class SmtpEmailService : IEmailService
    {
        public void SendEmail(string recipient, string subject, string body)
        {
            var credt = new NetworkCredential("issistem.task@gmail.com", "uifbgcvikkaocsou");
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = credt,
                EnableSsl = true
            };

            MailMessage mailMessage = new();
            mailMessage.To.Add(recipient);
            MailAddress address = new MailAddress("issistem.task@gmail.com");
            mailMessage.From = address;
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            smtpClient.Send(mailMessage);
        }
    }
}
