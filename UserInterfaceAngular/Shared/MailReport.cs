using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace ColloSys.UserInterface.Shared
{
    public class MailReport
    {
        /// <summary>
        /// To Send Mail
        /// </summary>
        /// <param name="body"></param>
        /// <param name="logger"></param>
        public static void SendMail(string body ,Logger logger)
        {
           var errorReportMailId = WebConfigurationManager.AppSettings["ErrorReportingMailAddress"];

            #region mail config
            var mail = new MailMessage();
            mail.To.Add(errorReportMailId);
            mail.From = new MailAddress("collosys@sc.com");

            mail.Subject = "Collosys error reporting " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            mail.IsBodyHtml = true;
            mail.Body = body;
            mail.Priority = MailPriority.High;
            #endregion

            #region SMTP config
            try
            {
                //TODO : generic by web config to send mail diff mailid 
                var smtpServer = ColloSysParam.WebParams.SmtpClient;
                smtpServer.Send(mail);
                smtpServer.Dispose();

                logger.Info("Call SendigMail");
            }
            catch (SmtpException e)
            {
                logger.Fatal("Error: {0}", e.StatusCode);
            }
            #endregion
        }

        public static void SendMail(string body, string mailTo)
        {
            var mail = new MailMessage();
            mail.To.Add(mailTo);
            mail.From = new MailAddress("collosys@sc.com");
            mail.Subject = "Stakeholder request approved.";
            mail.IsBodyHtml = true;
            mail.Body = body;
            mail.Priority = MailPriority.High;
            try
            {
                var smtpServer = ColloSysParam.WebParams.SmtpClient;
                smtpServer.Send(mail);
                smtpServer.Dispose();

            }
            catch (SmtpException e)
            {
            }
        }
    }
}