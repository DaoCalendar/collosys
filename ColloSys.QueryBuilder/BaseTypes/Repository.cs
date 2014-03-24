using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public abstract class Repository<T> : IRepository<T> where T : Entity, new()
    {
        [Transaction]
        public virtual IEnumerable<T> GetAll()
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<T>().List<T>();
        }

        [Transaction]
        public virtual T Get(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<T>().Where(x => x.Id == id).SingleOrDefault<T>();
        }

        [Transaction]
        public virtual IList<T> FilterBy(Expression<Func<T, bool>> expression)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<T>().Where(expression).List();
        }

        [Transaction(Persist = true)]
        public virtual T Save(T obj)
        {
            var session = SessionManager.GetCurrentSession();
            session.SaveOrUpdate(obj);
            return obj;
        }

        [Transaction(Persist = true)]
        public virtual void Save(IEnumerable<T> listOfObjects)
        {
            var session = SessionManager.GetCurrentSession();
            foreach (var obj in listOfObjects)
            {
                session.SaveOrUpdate(obj);
            }
        }

        [Transaction(Persist = true)]
        public virtual void Delete(T obj)
        {
            var session = SessionManager.GetCurrentSession();
            session.Delete(obj);
        }

        [Transaction(Persist = true)]
        public virtual T Merge(T obj)
        {
            var session = SessionManager.GetCurrentSession();
            obj = session.Merge(obj);
            return obj;
        }

        [Transaction]
        public virtual T Load(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            return session.Load<T>(id);
        }

        [Transaction]
        public virtual IEnumerable<T> Execute(QueryOver<T> query)
        {
            var session = SessionManager.GetCurrentSession();
            return query.GetExecutableQueryOver(session).List<T>();
        }
        public abstract QueryOver<T, T> ApplyRelations();

        [Transaction]
        public T Refresh(T obj)
        {
            var session = SessionManager.GetCurrentSession();
            session.Refresh(obj);
            return obj;
        }
    }
}
