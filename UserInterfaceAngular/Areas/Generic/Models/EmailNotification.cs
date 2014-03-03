using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace ColloSys.UserInterface.Areas.Generic.Models
{
    public class EmailNotification
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        public static bool SendNotificationEmail(string Email, string userName)
        {
            try
            {
                //var session = SessionManager.GetCurrentSession();
                //var getAllAllocation = session.QueryOver<AllocPolicy>().Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted).List();

                //var alloc = getAllAllocation.Select(x => x.Status == ColloSysEnums.ApproveStatus.Submitted).Count();
                
                //var getAllBillingPolicy = session.QueryOver<BillingPolicy>().Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted).List();//pending

                //var billing = getAllBillingPolicy.Select(x => x.Status == ColloSysEnums.ApproveStatus.Submitted).Count();

                //var mail = new MailMessage();
                //var smtpServer = ColloSysParam.WebParams.SmtpClient;
                //mail.To.Add(Email);
                //mail.Subject = "Collosys Pending Activities:-";
                //mail.Body = "Dear " + userName + ",You have below pending activities :-\n" +
                //    "\nAllocation Changes:-" + alloc +
                //    "\n\nBilling Changes :-" + billing +
                //    "\n\n For Login Collosys click here:-" + "http://collosys.cloudapp.net/new" +
                //    "\n\nRegards,\n" +
                //    "\nColloSys Support Team\n";

                //mail.Priority = MailPriority.High;
                //smtpServer.Send(mail);
                //_log.Info("End Of the Day Email Notification Send to " + userName);
                return true;

            }
            catch (SmtpException e)
            {
                _log.Error(e.Message);
                return false;
            }
        }

        public static string TemplateForApproveuser(string userName, string userId)
        {
            const string spanStart = "<span>";
            const string spanEnd = "</span>";
            const string br = "<br/>";
            const string space = " ";
            var body = "";

            //generate url
            var urlhelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var scheme = HttpContext.Current.Request.Url.Scheme; //urlhelper.RequestContext.HttpContext.Request.Url.Scheme;
            var url = urlhelper.Action("Login", "Account", null, scheme);

            body += spanStart + "Dear" + space + userName.ToUpperInvariant() + spanEnd + br + br;
            body += spanStart + "Your have been granted access to collosys." + spanEnd + br;
            body += spanStart + "The access details are mention below:" + spanEnd + br + br;
            body += spanStart + "UserID:" + space + "<b>" + userId + "</b>" + spanEnd + br;
            body += spanStart + "Password:" + space + "<b>collosys</b>" + spanEnd + br + br;
            body += spanStart + "You can login to collosys application with " + spanEnd + br;
            body += spanStart + "<a href=\"" + url + "\">Click Here</a>" + spanEnd + br + br;
            body += spanStart + "Regards" + spanEnd + br;
            body += spanStart + "ColloSys Support Team" + spanEnd;

            return body;
        }
    }
}