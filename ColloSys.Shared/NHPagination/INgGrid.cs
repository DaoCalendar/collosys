#region  references

using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;

#endregion

namespace ColloSys.Shared.NHPagination
{
    public interface INgGrid
    {
        DetachedCriteria BaseCriteria { get; set; }
        IPagedList Page { get; set; }
        IEnumerable<ColumnsDefinitions> ColumnsDefs { get; set; }
        IEnumerable<KeyValuePair<string, string>> SortInfo { get; set; }
        IEnumerable<KeyValuePair<string, string>> FilterInfo { get; set; }

        void ResetCriteria();
        IList FetchPage(uint pageNo, uint pageSize);
        void ApplySorting();
    }
}

//        void ApplySorting(string field, string value);
//void ApplyFiltering(string field, string value);
//void ApplySorting(string field, bool ascending);
