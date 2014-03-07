using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;
using NHibernate.Context;

namespace ColloSys.UserInterface.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpSessionAttribute : ActionFilterAttribute
    {
        protected ISession Session { get; private set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Session = SessionManager.BindNewSession();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            SessionManager.UnbindSession();
        }

    }
}