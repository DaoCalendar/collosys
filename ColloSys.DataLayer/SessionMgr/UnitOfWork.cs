#region References

using System;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Infra.SessionMgr
{
    public class UnitOfWork : IDisposable
    {
        public ISession CurrentSession { get; private set; }

        public UnitOfWork(ISession session)
        {
            CurrentSession = session;
            CurrentSession.BeginTransaction();
        }

        public void Commit()
        {
            if (CurrentSession.Transaction == null || !CurrentSession.Transaction.IsActive)
            {
                return;
            }

            CurrentSession.Transaction.Commit();
        }

        public void Dispose()
        {
            CurrentSession.Dispose();
        }
    }
}
