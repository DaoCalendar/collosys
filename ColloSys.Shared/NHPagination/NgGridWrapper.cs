#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;
using NHibernate.Criterion;

#endregion

namespace ColloSys.Shared.NHPagination
{
    public class NgGridWrapper : INgGrid
    {
        #region iface - props

        public IEnumerable<ColumnsDefinitions> ColumnsDefs { get; set; }

        public IPagedList Page { get; set; }

        public DetachedCriteria BaseCriteria { get; set; }

        public IEnumerable<KeyValuePair<string, string>> SortInfo { get; set; }

        public IEnumerable<KeyValuePair<string, string>> FilterInfo { get; set; }

        #endregion

        #region ctor

        private DetachedCriteria _detachedCriteria;
        private readonly Type _entityType;

        public NgGridWrapper(DetachedCriteria criteria, Type entityType)
        {
            //create sort-filter
            SortInfo = new List<KeyValuePair<string, string>>();
            FilterInfo = new List<KeyValuePair<string, string>>();

            // save original criteria
            BaseCriteria = CriteriaTransformer.Clone(criteria);
            _detachedCriteria = criteria;

            // get columns to display
            _entityType = entityType;
            ColumnsDefs = InitColumnDefs();

            // fetch first page
            Page = new PagedList();
            FetchFirstPage();
        }

        private IEnumerable<ColumnsDefinitions> InitColumnDefs()
        {
            // if file upload table, dont display collosys props
            IList<string> ignoredProps = new List<string>();
            if (typeof(IFileUploadable).IsAssignableFrom(_entityType))
            {
                var inst = Activator.CreateInstance(_entityType);
                var fileUploadObj = inst as IFileUploadable;
                if (fileUploadObj != null)
                {
                    ignoredProps = fileUploadObj.GetExcludeInExcelProperties();
                }
            }

            // get list of non-ignored props
            return _entityType.GetProperties()
                              .Where(x => !ignoredProps.Contains(x.Name))
                              .Select(x => new ColumnsDefinitions
                                  {
                                      CellFilter = string.Empty,
                                      DisplayName = x.Name,
                                      Field = x.Name
                                  })
                              .ToList();
        }

        private void FetchFirstPage(ISession session = null)
        {
            if (session == null) session = SessionManager.GetCurrentSession();
            Page.FetchPageData(_detachedCriteria, session, 1, 100);
        }

        #endregion

        #region iface - methods

        public void ResetCriteria()
        {
            _detachedCriteria = CriteriaTransformer.Clone(BaseCriteria);
        }

        public IList FetchPage(uint pageNo, uint pageSize)
        {
            Page.FetchPageData(_detachedCriteria, pageNo, pageSize);
            return Page.PageData;
        }

        public void ApplySorting()
        {
            foreach (var pair in SortInfo)
            {
                var descending = !string.IsNullOrWhiteSpace(pair.Value) &&
                                 pair.Value.ToUpperInvariant().Trim().Contains("DESC");
                _detachedCriteria.AddOrder(new Order(pair.Key, !descending));
            }
        }

        #endregion
    }
}


//public void ApplyFiltering(string field, string value)
//{
//    _detachedCriteria.Add(Restrictions.Like(field, value, MatchMode.Anywhere));
//}

//public void ApplySorting(string field, bool ascending)
//{
//    _detachedCriteria.AddOrder(new Order(field, ascending));
//}


