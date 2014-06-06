using System.Net.Mail;
using ColloSys.Shared.ConfigSectionReader;

namespace EMailServices
{
    public class Emailer
    {
        private readonly MailMessage _mailMessage;
        public Emailer(string emailto)
        {
            _mailMessage = new MailMessage("collosys@sc.com", emailto) {IsBodyHtml = false};
        }

        public void SetSubject(string subject)
        {
            _mailMessage.Subject = subject;
        }

        public void SetBody(string body)
        {
            _mailMessage.Body = body;
        }

        public void Send()
        {
            ColloSysParam.WebParams.SmtpClient.Send(_mailMessage);
        }
    }
}
