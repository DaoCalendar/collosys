using ColloSys.UserInterface.Shared;
using NLog;
using System;
using System.Web.Mvc;

namespace ColloSys.UserInterface.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [ValidateInput(false)]
        public ActionResult Index(Exception exception)
        {
            var error = new ErrorModel
                {
                    ExceptionType = exception.GetType().FullName,
                    ExceptionSummary = exception.Message,
                    ExceptionStackTrace = exception.StackTrace
                };

            return View("Index", error);
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(ErrorModel model)
        {
            if (model != null)
            {
                var body = GetBody(model);

                // mail sending 
                MailReport.SendMail(body, _logger);

                model.Message = " Error has been logged successfully. AlgoSys team will get in touch with you shortly.";

                _logger.Info("Error has been logged successfully. AlgoSys team will get in touch with you shortly.");
                return View("Index", model);
            }
            model = new ErrorModel
                {
                    Message = "Failed to report the error. Please send mail to scbsupport@algosystech.com"
                };
            _logger.Info("Failed to report the error. Please send mail to scbsupport@algosystech.com");

            return View("Index", model);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        private string GetBody(ErrorModel model)
        {
            var body = "<b>Issue Summary : </b>" + model.Summary + "<br/>" +
                       "<b>Priority      : </b>" + model.Priority + "<br/>" +
                       "<b>Error Type    : </b>" + model.ExceptionType + "<br/>" +
                       "<b>Error Message : </b>" + model.ExceptionSummary + "<br/>" +
                       "<b>Error Stack   : </b>" + model.ExceptionStackTrace;
            return body;
        }
    }
}

#region Mail sending by code
/*
   /// <summary>
        /// To Send Mail
        /// </summary>
        /// <param name="Body"></param>
        public void SendMail(string Body)//, string ToAddress)
        {
            var errorReportMailId = WebConfigurationManager.AppSettings["ErrorReportingMailAddress"];
            var mail = new MailMessage();
            var smtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("algosys.server@gmail.com");
            //mail.To.Add("info@algosystech.com");
            mail.To.Add(errorReportMailId);
            mail.Subject = "Collosys Error Reporting " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            mail.IsBodyHtml = true;
            mail.Body = Body;
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("algosys.server@gmail.com", "p@55w0rld");
            smtpServer.EnableSsl = true;
            try
            {
                smtpServer.Send(mail);
                _logger.Info("Call SendigMail");
            }
            catch (SmtpException e)
            {
                _logger.Fatal("Error: {0}", e.StatusCode);
            }
        }
 
 */
#endregion