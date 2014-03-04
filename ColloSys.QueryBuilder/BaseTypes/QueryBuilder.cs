using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public abstract class QueryBuilder<T> : IQueryBuilder<T> where T : Entity, new()
    {
        protected ISession Session = SessionManager.GetCurrentSession();

        public virtual IList<T> GetAll()
        {
            return Session.QueryOver<T>().List<T>();
        }

        public IList<T> GetAll(bool distinct)
        {
            var result = GetAll();
            if (!distinct)
                return result;

            return result.Distinct().ToList();
        }

        public virtual T GetWithId(Guid id)
        {
            return Session.QueryOver<T>().Where(x => x.Id == id).SingleOrDefault<T>();
        }

        public IList<T> GetOnExpression(Expression<Func<T,bool>>  expression)
        {
            return Session.QueryOver<T>().Where(expression).List();
        }

        public void Save(T obj)
        {
            using (var trans=Session.BeginTransaction())
            {
                Session.Save(obj);
                trans.Commit();
            }
        }
    }
}
