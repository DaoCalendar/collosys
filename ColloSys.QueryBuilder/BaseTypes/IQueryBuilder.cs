using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public interface IQueryBuilder<T>
    {
        IList<T> GetAll();
        IList<T> GetAll(bool distinct);
        T GetWithId(Guid id);
        IList<T> GetOnExpression(Expression<Func<T,bool>>  expression);
        void Save(T obj);
    }
}
