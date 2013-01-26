using System.Net.Mail;

namespace POSSIBLE.TokenBasedAccess
{
    public class SmtpNotificationService : INotificationService
    {
        public bool Send(string to, string subject, string body)
        {
            try
            {
                var email = new MailMessage {Subject = subject, Body = body};
                email.To.Add(to);
                
                var smtp = new SmtpClient();
                smtp.Send(email);
            }
            catch(SmtpException e)
            {
                return false;
            }

            return true;
        }
    }
}