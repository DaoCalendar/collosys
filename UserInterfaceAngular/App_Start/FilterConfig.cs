#region references

using ColloSys.UserInterface.Shared;
using System.Web;
using System.Web.Mvc;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

#endregion


namespace ColloSys.UserInterface.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new MvcSessionAttribute());
            //filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class ElmahHandledErrorLoggerFilter : IExceptionFilter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled) return;

            _logger.Error(" "+context.Exception.Message);
            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(context.Exception));
            MailReport.SendMail(GetBody(context), _logger);
            _logger.Error(context.Exception);
        }

        private static string GetBody(ExceptionContext context)
        {
            var body = "Exception Type          :- " + context.Exception.GetType()
                    + "<br/>Exception Message      :- " + context.Exception.Message
                    + "<br/>Exception StackTrace   :- " + context.Exception.StackTrace;

            return body;
        }
    }
}