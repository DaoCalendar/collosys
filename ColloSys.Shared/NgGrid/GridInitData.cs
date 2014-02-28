#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.NHPagination;
using NHibernate.Criterion;

#endregion

namespace ColloSys.Shared.NgGrid
{
    public class GridInitData
    {
        #region properties
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public GridQueryResult QueryResult { get; set; }
        public GridQueryParams QueryParams { get; set; }
        public ColloSysEnums.GridScreenName ScreenName { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        public GridInitData(DetachedCriteria criteria, Type entityType,
                            ColloSysEnums.GridScreenName screen = ColloSysEnums.GridScreenName.NotSpecified)
        {
            QueryParams = new GridQueryParams
                {
                    Criteria = SerializeCriteria(criteria),
                    CriteriaOnType = entityType.AssemblyQualifiedName,
                    GridConfig = new NgGridConfig { columnDefs = InitColumnDefs(entityType) }
                };
            QueryResult = FetchFirstPage(criteria);
            ScreenName = screen;
        }

        public GridInitData(GridQueryParams queryParams, ColloSysEnums.GridScreenName screen)
        {
            QueryParams = queryParams;
            QueryResult = FetchPageData(queryParams.GetCriteria());
            ScreenName = screen;
        }

        #region data helpers

        private byte[] SerializeCriteria(DetachedCriteria criteria)
        {
            var ms = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, criteria);
            return ms.GetBuffer();
        }


        private GridQueryResult FetchFirstPage(DetachedCriteria criteria)
        {
            IPagedList page = new PagedList();
            page.FetchPageData(criteria, QueryParams.GridConfig.pagingOptions.currentPage,
                               QueryParams.GridConfig.pagingOptions.pageSize);
            return new GridQueryResult { PageData = page.PageData, TotalRowCount = page.TotalCount };
        }

        private GridQueryResult FetchPageData(DetachedCriteria criteria)
        {
            IPagedList page = new PagedList();
            page.FetchPageData(criteria, QueryParams);
            return new GridQueryResult { PageData = page.PageData, TotalRowCount = page.TotalCount };
        }

        #endregion

        #region column defs

        private IList<NgGridColumn> InitColumnDefs(Type entityType)
        {
            // if file upload table, dont display collosys props
            List<string> ignoredProps = new List<string>();
            if (typeof(IFileUploadable).IsAssignableFrom(entityType))
            {
                var inst = Activator.CreateInstance(entityType);
                var fileUploadObj = inst as IFileUploadable;
                if (fileUploadObj != null)
                {
                    ignoredProps.AddRange(fileUploadObj.GetExcludeInExcelProperties());
                }
            }

            if (typeof(Entity).IsAssignableFrom(entityType))
            {
                ignoredProps.AddRange(typeof(Entity).GetProperties().Select(x => x.Name));
            }

            // get list of non-ignored props
            return (from property in entityType.GetProperties()
                                               .Where(x => !ignoredProps.Contains(x.Name)
                                                           && !typeof(Entity).IsAssignableFrom(x.PropertyType))
                    let coltype = GetJsType(property)
                    select new NgGridColumn
                        {
                            displayName = property.Name,
                            field = property.Name,
                            cellType = coltype,
                            cellFilter = GetColumnFilter(coltype),
                            cellClass = GetColumnClass(coltype),
                            width = GetColumnWidth(coltype)
                        }).ToList();
        }

        public void AddNewColumn(PropertyInfo property, string typeName = "", string displayName = "")
        {
            var coltype = GetJsType(property);
            var ngGridColumn = new NgGridColumn
                {
                    displayName = string.IsNullOrWhiteSpace(displayName) ? property.Name : displayName,
                    field = string.IsNullOrWhiteSpace(typeName) ? property.Name : string.Format("{0}.{1}", typeName, property.Name),
                    cellType = coltype,
                    cellFilter = GetColumnFilter(coltype),
                    cellClass = GetColumnClass(coltype),
                    width = GetColumnWidth(coltype)
                };

            QueryParams.GridConfig.columnDefs.Add(ngGridColumn);
        }

        private NgGridColumn GetNgGridColumn(PropertyInfo property)
        {
            var coltype = GetJsType(property);
            return new NgGridColumn
                 {
                     displayName = property.Name,
                     field = property.Name,
                     cellType = coltype,
                     cellFilter = GetColumnFilter(coltype),
                     cellClass = GetColumnClass(coltype),
                     width = GetColumnWidth(coltype)
                 };
        }

        private string GetColumnFilter(JsTypes jsTypes)
        {
            switch (jsTypes)
            {
                case JsTypes.Amount:
                    return "currency";
                case JsTypes.Date:
                    return "date:'dd-MMM-yyyy'";
                case JsTypes.DateTime:
                    return "date:'dd-MMM-yyyy HH:mm:ss'";
                default:
                    return string.Empty;
            }
        }

        private string GetColumnClass(JsTypes jsTypes)
        {
            switch (jsTypes)
            {
                case JsTypes.Number:
                case JsTypes.Amount:
                    return "text-right";
                case JsTypes.Date:
                case JsTypes.DateTime:
                case JsTypes.Bool:
                    return "text-center";
                default:
                    return string.Empty;
            }
        }

        private ushort GetColumnWidth(JsTypes jsTypes)
        {
            switch (jsTypes)
            {
                case JsTypes.Date:
                case JsTypes.Number:
                case JsTypes.Bool:
                    return 100;
                default:
                    return 150;
            }
        }

        private JsTypes GetJsType(PropertyInfo property)
        {
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (!propertyType.IsValueType)
            {
                return JsTypes.Text;
            }

            if (propertyType == typeof(decimal))
            {
                return JsTypes.Amount;
            }

            if (propertyType == typeof(bool))
            {
                return JsTypes.Bool;
            }

            if (propertyType.IsEnum)
            {
                return JsTypes.Enum;
            }

            if (propertyType == typeof(DateTime))
            {
                string[] list = { "CreatedOn", "ApprovedOn" };
                if (list.Any(x => x == property.Name) || (property.Name.ToUpperInvariant().EndsWith("DATETIME")))
                {
                    return JsTypes.DateTime;
                }
                if (property.Name.ToUpperInvariant().EndsWith("DATE"))
                {
                    return JsTypes.Date;
                }
            }

            IList<Type> numberTypes = new List<Type>
                {
                    typeof (ushort),
                    typeof (short),
                    typeof (int),
                    typeof (uint),
                    typeof (long),
                    typeof (ulong)
                };
            if (numberTypes.Contains(propertyType))
            {
                return JsTypes.Number;
            }

            return JsTypes.Text;
        }

        #endregion
    }
}
