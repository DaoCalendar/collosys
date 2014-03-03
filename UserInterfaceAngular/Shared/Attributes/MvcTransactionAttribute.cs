#region references

using System;
using System.Web.Mvc;
using ColloSys.DataLayer.Infra.SessionMgr;

#endregion

namespace ColloSys.UserInterface.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MvcTransactionAttribute : MvcSessionAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            SessionManager.GetCurrentSession().BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var session = SessionManager.GetCurrentSession();
            var tx = session.Transaction;
            if (tx != null && tx.IsActive)
            {
                session.Transaction.Commit();
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
