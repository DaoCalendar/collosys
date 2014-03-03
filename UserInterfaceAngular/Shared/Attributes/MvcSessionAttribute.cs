#region references

using System;
using System.Web.Mvc;
using ColloSys.DataLayer.Infra.SessionMgr;

#endregion

namespace ColloSys.UserInterface.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MvcSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SessionManager.BindNewSession();
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            SessionManager.UnbindSession();
        }
    }
}