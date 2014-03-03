#region references

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using ColloSys.DataLayer.Infra.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using UserInterfaceAngular.Areas.Reporting.ViewModels;

#endregion

namespace ColloSys.UserInterface.Areas.Reporting.apiController
{
    public class NlogController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public ReportWrapper Get(int pageSize, int currentPage)
        {
            var session = SessionManager.GetCurrentSession();
            var rw = new ReportWrapper
                {
                    totalRecords = session.Query<NlogDb>().Count().ToString(CultureInfo.InvariantCulture),
                    DataList = session.Query<NlogDb>()
                               .OrderByDescending(x => x.nlogID)
                               .Skip((currentPage - 1) * pageSize)
                               .Take(pageSize)
                };

            var propertyInfos = typeof(NlogDb).GetProperties();
            rw.Columns = getColumns(propertyInfos);
            rw.GridOptions = getGridOptions();

            return rw;
        }

        private IList<GridOptions> getGridOptions()
        {
            IList<GridOptions> gridOptions = new List<GridOptions>();
            var grdOptions = new GridOptions
            {
                aggregateTemplate = "",
                checkboxCellTemplate = "",
                checkboxHeaderTemplate = "",
                enableCellEdit = false,
                enableCellSelection = false,
                enableColumnReordering = true,
                enableColumnResize = false,
                enableHighlighting = false,
                enablePaging = true,
                enableRowReordering = false,
                enableRowSelection = false,
                enableSorting = true,
                footerRowHeight = "55",
                plugins = "[plugins.ngGridLayoutPlugin]",
                groups = "[]",
                groupsCollapsedByDefault = true,
                headerRowHeight = "32",
                jqueryUIDraggable = false,
                jqueryUITheme = false,
                keepLastSelected = true,
                multiSelect = false,
                pagingOptions = "{ pageSizes: [250, 500, 1000], pageSize: 250, totalServerItems: 0, currentPage: 1 }",
                rowHeight = "30",
                rowTemplate = "",
                selectWithCheckboxOnly = false,
                showColumnMenu = false,
                showFilter = false,
                showFooter = true,
                tabIndex = "0"
            };

            gridOptions.Add(grdOptions);

            return gridOptions;
        }

        private IList<Columns> getColumns(IEnumerable<PropertyInfo> propertyInfo)
        {
            return propertyInfo.Select(t => new Columns
                {
                    field = t.Name,
                    type = "text",
                    aggLabelFilter = "",
                    cellClass = "",
                    cellFilter = "",
                    cellTemplate = "",
                    displayName = t.Name,
                    editableCellTemplate = "",
                    enableCellEdit = true,
                    groupable = true,
                    headerCellTemplate = "",
                    headerClass = "",
                    maxWidth = "9000",
                    minWidth = "50",
                    pinnable = true,
                    resizable = true,
                    sortable = true,
                    width = "*"
                }).ToList();
        }
    }
}
