using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ColloSys.DataLayer.BaseEntity;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetWithId(Guid id);
        T Load(Guid id);
        IList<T> GetOnExpression(Expression<Func<T, bool>> expression);
        void Save(T obj);
        void Save(IEnumerable<T> listOfObjects);
        void Save(Entity entity);
        void Delete(T obj);
        void Merge(T obj);
        void Merge(Entity entity);
        IEnumerable<T> ExecuteQuery(QueryOver<T> query);
        QueryOver<T,T> WithRelation();
        void Refresh(T obj);

        //IQueryOver<T> GetDefaultQuery { get;}
    }
}
