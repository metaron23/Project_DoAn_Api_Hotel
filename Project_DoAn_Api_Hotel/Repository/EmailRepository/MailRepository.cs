using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Project_DoAn_Api_Hotel.Model;
using System.Net;

namespace Project_DoAn_Api_Hotel.Repository.EmailRepository
{
    public class MailRepository : IMailRepository
    {
        private readonly IConfiguration _configuration;

        public MailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Email(EmailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(mailRequest.To));
            email.Subject = mailRequest.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = mailRequest.Body
            };
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailUserName").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
