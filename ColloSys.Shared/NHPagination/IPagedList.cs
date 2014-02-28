#region references

using System.Collections;
using ColloSys.Shared.NgGrid;
using NHibernate;
using NHibernate.Criterion;

#endregion

namespace ColloSys.Shared.NHPagination
{
    public interface IPagedList
    {
        IList PageData { get; }
        uint PageIndex { get; }
        uint PageSize { get; }
        ulong TotalCount { get; }
        uint TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }

        //IList FetchPageData(ICriteria criteria, uint pageIndex, uint pageSize);
        IList FetchPageData(DetachedCriteria detachedCriteria, uint pageIndex, uint pageSize);
        void FetchPageData(DetachedCriteria detachedCriteria, GridQueryParams param);
        void FetchAllData(DetachedCriteria detachedCriteria, GridQueryParams param);
        //IList FetchPageData(DetachedCriteria detachedCriteria, ISession session, uint pageIndex, uint pageSize);
    }
}


//public interface IPagedList
//{
//    IList PageData { get; }
//    uint PageIndex { get; }
//    uint PageSize { get; }
//    ulong TotalCount { get; }
//    uint TotalPages { get; }
//    bool HasPreviousPage { get; }
//    bool HasNextPage { get; }

//    void FetchPageData(ICriteria criteria, uint pageIndex, uint pageSize);
//}

//public interface IPagedList2
//{
//    ICriteria Criteria { get; }
//    IList PageData { get; }
//    uint PageIndex { get; }
//    uint PageSize { get; }
//    ulong TotalCount { get; }
//    uint TotalPages { get; }
//    bool HasPreviousPage { get; }
//    bool HasNextPage { get; }

//    void FetchPageData(uint pageIndex, uint pageSize);
//}
