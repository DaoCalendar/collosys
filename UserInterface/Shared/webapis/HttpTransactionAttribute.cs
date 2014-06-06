using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ColloSys.DataLayer.SessionMgr;

namespace AngularUI.Shared.webapis
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HttpTransaction2Attribute : ActionFilterAttribute
    {
        public HttpTransaction2Attribute()
        {
            Persist = false;
        }

        public bool Persist { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            SessionManager.GetCurrentSession().BeginTransaction();
        }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            var tx = SessionManager.GetCurrentSession().Transaction;
            if (tx != null && tx.IsActive)
            {
                if (Persist)
                {
                    tx.Commit();
                }
                else
                {
                    tx.Rollback();
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}