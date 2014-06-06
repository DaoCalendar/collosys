using System;
using ColloSys.DataLayer.SessionMgr;
using NHibernate;
using PostSharp.Aspects;

namespace ColloSys.QueryBuilder.TransAttributes
{
    [Serializable]
    public class SessionAttribute : OnMethodBoundaryAspect
    {
        protected ISession Session;

        public override void OnEntry(MethodExecutionArgs args)
        {
            Session = SessionManager.BindNewSession();
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            SessionManager.UnbindSession();
        }
    }
}
