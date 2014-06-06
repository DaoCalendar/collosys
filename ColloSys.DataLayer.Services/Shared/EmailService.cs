#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Linq;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NHibernate.Linq;
using NLog;

#endregion

namespace ColloSys.DataLayer.Services.Shared
{
    public static class EmailService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public readonly static SmtpClient SmtpClient;
        public readonly static uint EmailSendBatchSize;

        static EmailService()
        {
            SmtpClient = ColloSysParam.WebParams.SmtpClient;
            EmailSendBatchSize = 100;
        }

        public static void EmailReport(FileInfo file, string emailId, string subject)
        {
            if (string.IsNullOrWhiteSpace(emailId)) return;
            var message = new MailMessage { From = new MailAddress("collosys@sc.com", "ColloSys") };
            message.To.Add(new MailAddress(emailId));
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = subject;
            message.Attachments.Add(new Attachment(file.FullName));
            message.Body = @"Thanks, ColloSys Team.";

            try
            {
                SmtpClient.Send(message);
            }
            catch(Exception exception)
            {
                _logger.LogException(LogLevel.Fatal, "Could not send mail", exception);
            }
        }

        public static void EmailReport(FileInfo file,string subject,List<string> emailIds)
        {
            var batchSize = (int) EmailSendBatchSize;
            for (var i = 0; i < emailIds.Count(); i += batchSize)
            {
                var batchEmailId = emailIds.Skip(i).Take(batchSize);
                
                var message = new MailMessage { From = new MailAddress("collosys@sc.com", "ColloSys") };
                batchEmailId.ForEach(x => message.To.Add(new MailAddress(x)));
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = subject;
                message.Attachments.Add(new Attachment(file.FullName));
                message.Body = @"Thanks, ColloSys Team.";

                try
                {
                    SmtpClient.Send(message);
                }
                catch (Exception exception)
                {
                    _logger.LogException(LogLevel.Fatal, "Could not send mail", exception);
                }
            }
               
        }

        public static string GetUserEmail(string username)
        {
            var session = SessionManager.GetCurrentSession();
            var emailId = session.QueryOver<GUsers>()
                .Where(x => x.Username == username)
                .Select(x => x.Email).SingleOrDefault<string>();
            return emailId ?? string.Empty;
        }
    }
}


//client.SendCompleted += (sender, error) =>
//{
//    //if (error.Error != null) { }
//    client.Dispose();
//    message.Dispose();
//};
////ThreadPool.QueueUserWorkItem(o => client.SendAsync(message, Tuple.Create(client, message)));
//client.SendAsync(message, Guid.NewGuid());

