using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public interface IQueryBuilder<T>
    {
        IEnumerable<T> GetAll();
        //IList<T> GetAll(bool distinct);
        T GetWithId(Guid id);
        IList<T> GetOnExpression(Expression<Func<T,bool>>  expression);
        void Save(T obj);
        void SaveList(IList<T> listOfObjects);
        void Delete(T obj);
        T Load(Guid id);
        T Merge(T obj);
        IEnumerable<T> ExecuteQuery(QueryOver<T> query);
        QueryOver<T,T> DefaultQuery();

        //IQueryOver<T> GetDefaultQuery { get;}
    }
}
