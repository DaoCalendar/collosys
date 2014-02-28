using System.Collections.Generic;
using System.IO;
using NHibernate;
using NHibernate.Criterion;

namespace ColloSys.Shared.NHPagination
{
    public class QueryOverGeneric<TEntity>
    {
        private IQueryOver<TEntity> Query { get; set; }
        private int PageSize { get; set; }
        public QueryOverGeneric(uint pagesize = 100)
        {
            Query = QueryOver.Of<TEntity>();
            ChangePageSize(pagesize);
        }

        private void ChangePageSize(uint pagesize)
        {
            if (pagesize == 0)
            {
                throw new InvalidDataException("Page size cannot be zero.");
            }
            if (pagesize > 2000)
            {
                throw new InvalidDataException("Page size should not be more than 2000.");
            }

            PageSize = (int)pagesize;
        }

        public IList<TEntity> GetPage(int pageno)
        {
            return Query.Skip((pageno - 1) * PageSize).Take(PageSize).List<TEntity>();
        }

        public QueryOverGeneric<TEntity> SortBy(string fieldName, bool asc = true)
        {
            ICriteria crit = Query.UnderlyingCriteria;
            crit.AddOrder(new Order(fieldName, asc));
            return this;
        } 


    }

    //public static class CriteriaPaging
    //{
    //    public static DetachedCriteria Build<T>(this IEnumerable<ICriterion> list)
    //    {
    //        var criteria = DetachedCriteria.For<T>();
    //        foreach (var criterion in list)
    //        {
    //            criteria.Add(criterion);
    //        }
    //        return criteria;
    //    }

    //    public static DetachedCriteria Page(this DetachedCriteria criteria, int pageNumber, int pageSize)
    //    {
    //        criteria.SetMaxResults(pageSize);
    //        criteria.SetFirstResult(pageSize * pageNumber - 1);
    //        return criteria;
    //    }

    //    public static DetachedCriteria OrderBy(this DetachedCriteria criteria, string fieldName, Direction direction)
    //    {
    //        criteria.AddOrder(new Order(fieldName, direction.IsAscending()));
    //        return criteria;
    //    }
    //}
}
