using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ColloSys.DataLayer.BaseEntity;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public interface IRepository<T> where T : Entity
    {
        IEnumerable<T> GetAll();
        T GetWithId(Guid id);
        T Load(Guid id);
        IList<T> GetOnExpression(Expression<Func<T, bool>> expression);
        T Save(T obj);
        void Save(IEnumerable<T> listOfObjects);
        void Delete(T obj);
        T Merge(T obj);
        IEnumerable<T> ExecuteQuery(QueryOver<T> query);
        QueryOver<T, T> WithRelation();
        T Refresh(T obj);
    }
}
