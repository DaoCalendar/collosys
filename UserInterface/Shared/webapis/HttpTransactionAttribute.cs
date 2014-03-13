using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;

namespace ColloSys.UserInterface.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HttpTransactionAttribute : HttpSessionAttribute
    {
        public HttpTransactionAttribute()
        {
            Persist = false;
        }

        public bool Persist { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            Session.BeginTransaction();
        }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            var tx = Session.Transaction;
            if (tx != null && tx.IsActive)
            {
                if (Persist)
                {
                    tx.Commit();
                    return;
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