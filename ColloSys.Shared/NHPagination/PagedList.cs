#region references

using System.Collections;
using ColloSys.Shared.NgGrid;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion

namespace ColloSys.Shared.NHPagination
{
    public class PagedList : IPagedList
    {
        #region properties

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public IList PageData { get; private set; }
        public uint PageIndex { get; private set; }
        public uint PageSize { get; private set; }
        public ulong TotalCount { get; private set; }
        public uint TotalPages { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        public PagedList()
        {
            PageIndex = 1;
            PageSize = 100;
        }

        #endregion

        #region data get methods

        public IList FetchPageData(DetachedCriteria detachedCriteria, uint pageIndex, uint pageSize)
        {
            // correct page params
            if (pageIndex != 0) PageIndex = pageIndex;
            if (pageSize != 0) PageSize = pageSize;

            // fetch totals
            TotalCount = NhCriteriaGenerator.GetTotalRowCount(detachedCriteria);
            TotalPages = NhCriteriaGenerator.GetTotalPageCount(TotalCount, PageSize);

            // fetch page data
            detachedCriteria = NhCriteriaGenerator.AddPaging(detachedCriteria, PageIndex, PageSize);
            detachedCriteria = detachedCriteria.SetResultTransformer(Transformers.DistinctRootEntity);
            PageData = NhCriteriaGenerator.GetExecutableCriteria(detachedCriteria).List();
            return PageData;
        }

        public void FetchPageData(DetachedCriteria detachedCriteria, GridQueryParams param)
        {
            // apply filtering
            if (param.FiltersList.Count > 0)
            {
                detachedCriteria = NhCriteriaGenerator.AddValueFiltering(detachedCriteria, param.GetCriteriaType(),
                                                                         param);
                detachedCriteria = NhCriteriaGenerator.AddFieldFiltering(detachedCriteria, param.GetCriteriaType(),
                                                                         param);
                detachedCriteria = NhCriteriaGenerator.AddRelativeFiltering(detachedCriteria, param.GetCriteriaType(),
                                                                            param);
            }
            // apply sorting
            for (var i = 0; i < param.GridConfig.sortInfo.fields.Count; i++)
            {
                detachedCriteria = NhCriteriaGenerator.AddOrdering
                    (detachedCriteria,
                     param.GetCriteriaType().Name + "." + param.GridConfig.sortInfo.fields[i],
                     param.GridConfig.sortInfo.directions[i]);
            }

            // get paged data
            FetchPageData(detachedCriteria, param.GridConfig.pagingOptions.currentPage, param.GridConfig.pagingOptions.pageSize);
        }

        public void FetchAllData(DetachedCriteria detachedCriteria, GridQueryParams param)
        {
            // apply filtering
            detachedCriteria = NhCriteriaGenerator.AddValueFiltering(detachedCriteria, param.GetCriteriaType(), param);

            // apply sorting
            for (var i = 0; i < param.GridConfig.sortInfo.fields.Count; i++)
            {
                detachedCriteria = NhCriteriaGenerator.AddOrdering
                    (detachedCriteria,
                     param.GetCriteriaType().Name + "." + param.GridConfig.sortInfo.fields[i],
                     param.GridConfig.sortInfo.directions[i]);
            }

            // get paged data
            detachedCriteria = detachedCriteria.SetResultTransformer(Transformers.DistinctRootEntity);
            PageData = NhCriteriaGenerator.GetExecutableCriteria(detachedCriteria).List();
        }
        #endregion

        #region has next/prev

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }

        #endregion
    }
}
