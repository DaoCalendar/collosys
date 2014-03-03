using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Areas.Reporting.ViewModels;
using NHibernate.Linq;

namespace UserInterfaceAngular.app
{
    public class CustomerDataNewApiController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> GetSystemCategory()
        {
            return new string[] { "File Details", "File Columns", "Cacs Activity", "Creditcard Info", "Creditcard Liner", "Creditcard Payment", "Creditcard Unbilled", "Creditcard WriteOff", "RLS Info", "RLS Liner", "RLS Payment", "RLS WriteOff", "EBBS Info", "EBBS Liner", "EBBS Payment", "EBBS WriteOff" };
            //return new string[] { "CACS Activity", "CCMS Liner", "CCMS Unbilled", "CCMS Writeoff", "CCMS Payment", "RLS Liner", "RLS Writeoff", "RLS Payment", "RLS Reversal", "EBBS Liner", "EBBS Writeoff", "EBBS Payment" };
        }       

        [HttpGet]
        [HttpTransaction]
        public ReportWrapper Get(string aliasName, int pageSize, int currentPage)
        {
            var session = SessionManager.GetCurrentSession();
            PropertyInfo[] propertyInfos;
            ReportWrapper RW = new ReportWrapper();

            switch (aliasName.ToUpper())
            {
                case "FILE DETAILS":
                    RW.totalRecords = session.Query<FileDetail>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<FileDetail>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(FileDetail).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "FILE COLUMNS":
                    RW.totalRecords = session.Query<FileColumn>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<FileColumn>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(FileColumn).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "CACS ACTIVITY":
                    RW.totalRecords = session.Query<CacsActivity>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<CacsActivity>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(CacsActivity).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
              
                case "CREDITCARD LINER":
                    RW.totalRecords = session.Query<CLiner>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<CLiner>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(CLiner).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "CREDITCARD PAYMENT":
                    RW.totalRecords = session.Query<CPayment>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<CPayment>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(CPayment).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "CREDITCARD UNBILLED":
                    RW.totalRecords = session.Query<CUnbilled>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<CUnbilled>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(CUnbilled).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "CREDITCARD WRITEOFF":
                    RW.totalRecords = session.Query<CWriteoff>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<CWriteoff>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(CWriteoff).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
               
                case "RLS LINER":
                    RW.totalRecords = session.Query<RLiner>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<RLiner>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(RLiner).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "RLS PAYMENT":
                    RW.totalRecords = session.Query<RPayment>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<RPayment>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(RPayment).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "RLS WRITEOFF":
                    RW.totalRecords = session.Query<RWriteoff>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<RWriteoff>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(RWriteoff).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
               
                case "EBBS LINER":
                    RW.totalRecords = session.Query<ELiner>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<ELiner>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(ELiner).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "EBBS PAYMENT":
                    RW.totalRecords = session.Query<EPayment>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<EPayment>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(EPayment).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
                case "EBBS WRITEOFF":
                    RW.totalRecords = session.Query<EWriteoff>().Count().ToString();// .RowCount().ToString();
                    RW.DataList = session.Query<EWriteoff>().Skip((currentPage - 1) * pageSize).Take(pageSize);
                    propertyInfos = typeof(EWriteoff).GetProperties();
                    RW.Columns = getColumns(propertyInfos);
                    break;
            }

            RW.GridOptions = getGridOptions();

            return RW;
        }


        public static ReportWrapper GetReportWrapper<T>(IQueryable<T> queryable, int pageSize, int currentPage) where T : Entity
        {
            ReportWrapper RW = new ReportWrapper();
            RW.totalRecords = queryable.Count().ToString();
            RW.DataList = queryable.Skip((currentPage - 1) * pageSize).Take(pageSize);
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            RW.Columns = getColumns(propertyInfos);

            RW.GridOptions = getGridOptions();

            return RW;
        }

        // GET api/reportapi
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        private static IList<GridOptions> getGridOptions()
        {
            IList<GridOptions> gridOptions = new List<GridOptions>();
            GridOptions grdOptions = new GridOptions();
            grdOptions.aggregateTemplate = "";
            grdOptions.checkboxCellTemplate = "";
            grdOptions.checkboxHeaderTemplate = "";
            grdOptions.enableCellEdit = false;
            grdOptions.enableCellSelection = false;
            grdOptions.enableColumnReordering = true;
            grdOptions.enableColumnResize = false;
            grdOptions.enableHighlighting = false;
            grdOptions.enablePaging = true;
            grdOptions.enableRowReordering = false;
            grdOptions.enableRowSelection = false;
            grdOptions.enableSorting = true;
            grdOptions.footerRowHeight = "55";
            grdOptions.plugins = "[plugins.ngGridLayoutPlugin]";
            grdOptions.groups = "[]";
            grdOptions.groupsCollapsedByDefault = true;
            grdOptions.headerRowHeight = "32";
            grdOptions.jqueryUIDraggable = false;
            grdOptions.jqueryUITheme = false;
            grdOptions.keepLastSelected = true;
            grdOptions.multiSelect = false;
            grdOptions.pagingOptions = "{ pageSizes: [250, 500, 1000], pageSize: 250, totalServerItems: 0, currentPage: 1 }";
            grdOptions.rowHeight = "30";
            grdOptions.rowTemplate = "";
            grdOptions.selectWithCheckboxOnly = false;
            grdOptions.showColumnMenu = false;
            grdOptions.showFilter = false;
            grdOptions.showFooter = true;
            grdOptions.tabIndex = "0";

            gridOptions.Add(grdOptions);

            return gridOptions;
        }

        private static IList<Columns> getColumns(PropertyInfo[] propertyInfo)
        {
            IList<Columns> columns = new List<Columns>();
            for (int _prop = 0; _prop < propertyInfo.Length; _prop++)
            {
                Columns c = new Columns();
                c.field = propertyInfo[_prop].Name;
                c.type = "text";// Algosystech.Common.getJSType(propertyInfo[_prop].PropertyType.Name);
               // c.aggLabelFilter = "";
                c.cellClass = "";
                c.cellFilter = "";
                c.cellTemplate = "";
                c.displayName = propertyInfo[_prop].Name;
                c.editableCellTemplate = "";
                c.enableCellEdit = true;
                c.groupable = true;
                c.headerCellTemplate = "";
                c.headerClass = "";
                c.maxWidth = "9000";
                c.minWidth = "50";
                c.pinnable = true;
                c.resizable = true;
                c.sortable = true;
                //c.sortFn = "";
                c.width = "*";

                columns.Add(c);
            }
            //            Columns column
            return columns;
        }

    }
}