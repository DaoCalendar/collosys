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
        T Get(Guid id);
        T Load(Guid id);
        T Save(T obj);
        void Save(IEnumerable<T> listOfObjects);
        void Delete(T obj);

        T Merge(T obj);
        T Refresh(T obj);

        IList<T> FilterBy(Expression<Func<T, bool>> expression);
        IEnumerable<T> Execute(QueryOver<T> query);
        QueryOver<T, T> ApplyRelations();
    }
}
