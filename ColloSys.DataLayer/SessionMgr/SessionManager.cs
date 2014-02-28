#region  references
using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.NhSetup;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using ColloSys.DataLayer.SessionMgr;
#endregion

namespace ColloSys.DataLayer.Infra.SessionMgr
{
    public static class SessionManager
    {
        #region  config

        public static void InitNhibernate(NhInitParams obj)
        {
            SessionFactoryManager.InitFactory(obj);
        }

        public static Configuration GetNhConfiguration()
        {
            return SessionFactoryManager.NhConfiguration;
        }

        public static ISessionFactory GetSessionFactory()
        {
            return SessionFactoryManager.SessionFactory;
        }

        #endregion

        #region sessions

        public static ISession BindNewSession()
        {
            var session = GetSessionFactory().OpenSession();
            session.FlushMode = FlushMode.Commit;
            CurrentSessionContext.Bind(session);
            return session;
        }

        public static void UnbindSession()
        {
            var session = CurrentSessionContext.Unbind(GetSessionFactory());
            if (session == null) return;
            session.Dispose();
        }

        public static ISession GetCurrentSession()
        {
            return GetSessionFactory().GetCurrentSession();
        }

        public static ISession GetNewSession()
        {
            return GetSessionFactory().OpenSession();
        }

        public static IStatelessSession GetStatelessSession()
        {
            return SessionFactoryManager.SessionFactory.OpenStatelessSession();
        }

        #endregion

        //#region dao/uow

        //public static UnitOfWork GetUnitOfWork()
        //{
        //    return new UnitOfWork(GetSessionFactory().OpenSession());
        //}

        public static NhRepository<T> GetRepository<T>()
            where T : Entity, new()
        {
            return new NhRepository<T>(GetCurrentSession());
        }

        //#endregion
    }
}