using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AngularUI.Shared.webapis
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HttpTransaction2Attribute : HttpSession2Attribute
    {
        public HttpTransaction2Attribute()
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
                tx.Rollback();
            }

            base.OnActionExecuted(filterContext);
        }
    }
}