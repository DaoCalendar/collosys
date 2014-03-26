using System;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;
using PostSharp.Aspects;

namespace ColloSys.QueryBuilder.TransAttributes
{
    [Serializable]
    public class TransactionAttribute:OnMethodBoundaryAspect
    {
        public bool Persist { get; set; }
        private ITransaction _transaction;

        public TransactionAttribute()
        {
            Persist = false;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var session = SessionManager.GetCurrentSession();
            _transaction = session.BeginTransaction();
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            if (_transaction == null || !_transaction.IsActive)
            {
                return;
            }

            if (Persist)
            {
                _transaction.Commit();
            }
            else
            {
                _transaction.Rollback();
            }
        }
    }
}