using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public interface IQueryBuilder<T>
    {
        IList<T> GetAll();
        //IList<T> GetAll(bool distinct);
        T GetWithId(Guid id);
        IList<T> GetOnExpression(Expression<Func<T,bool>>  expression);
        void Save(T obj);

        IQueryOver<T> DefaultQuery(ISession session);

        //IQueryOver<T> GetDefaultQuery { get;}
    }
}
