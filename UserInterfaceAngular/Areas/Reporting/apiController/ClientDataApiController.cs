using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Providers.Entities;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate;
using NHibernate.Criterion;
using UserInterfaceAngular.Areas.Reporting.ViewModels;
using NHibernate.Linq;
using ColloSys.UserInterface.Areas.Reporting.ViewModels;
using System;

namespace ColloSys.UserInterface.Areas.ClientData
{
    public class ClientDataApiController : ApiController
    {
        public IEnumerable<string> GetNames()
        {
            return new[] { 
                "Cacs Activity", 
                "Credit Card Liner", 
                "Credit Card Payment", 
                "Credit Card WriteOff", 
                "RLS Liner", 
                "RLS Payment", 
                "RLS WriteOff", 
                "EBBS Liner", 
                "EBBS Payment", 
                "EBBS WriteOff" 
            };
        }

        [HttpPost]
        [HttpTransaction]
        public ReportFilterWrapper GetFilter(int pageSize, int currentPage, string aliasName,string date)
        {
            var rw = new ReportFilterWrapper();

           // if (filter != null && filter.Length > 0) BuildHql(filter);
            string value;
            switch (aliasName.ToUpper())
            {
                case "FILE DETAILS":
                    rw.DataList = GetDataList("FileDetail", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(FileDetail).GetProperties());
                    break;
                case "FILE COLUMNS":
                    rw.DataList = GetDataList("FileColumn", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(FileColumn).GetProperties());
                    break;
                case "STAKE_HIERARCHY":
                    rw.DataList = GetDataList("StakeHierarchy", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(StkhHierarchy).GetProperties());
                    break;
                case "CACS ACTIVITY":
                    rw.DataList = GetDataList("CacsActivity", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(CacsActivity).GetProperties());
                    break;
                case "CREDIT CARD LINER":
                    rw.DataList = GetDataList("CLiner", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(CLiner).GetProperties());
                    break;
                case "CREDIT CARD PAYMENT":
                    rw.DataList = GetDataList("CPayment", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(CPayment).GetProperties());
                    break;
                case "CREDIT CARD UNBILLED":
                    rw.DataList = GetDataList("CUnbilled", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(CUnbilled).GetProperties());
                    break;
                case "CREDIT CARD WRITEOFF":
                    rw.DataList = GetDataList("CWriteoff", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(CWriteoff).GetProperties());
                    break;
                case "RLS LINER":
                    rw.DataList = GetDataList("RLiner", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(RLiner).GetProperties());
                    break;
                case "RLS PAYMENT":
                    rw.DataList = GetDataList("RPayment", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(RPayment).GetProperties());
                    break;
                case "RLS WRITEOFF":
                    rw.DataList = GetDataList("RWriteoff", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(RWriteoff).GetProperties());
                    break;
                case "EBBS LINER":
                    rw.DataList = GetDataList("ELiner", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(ELiner).GetProperties());
                    break;
                case "EBBS PAYMENT":
                    rw.DataList = GetDataList("EPayment", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(EPayment).GetProperties());
                    break;
                case "EBBS WRITEOFF":
                    rw.DataList = GetDataList("EWriteoff", pageSize, currentPage,date, out value);
                    rw.totalRecords = value;
                    rw.Columns = getColumns(typeof(EWriteoff).GetProperties());
                    break;
            }

            return rw;
        }

        private IEnumerable<dynamic> GetDataList(string tableName, int pageSize, int currentPage,string date, out string recordNumbers)
        {
         //   var filter = BuildFilter();
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                IEnumerable<dynamic> dataList =
                    session.CreateQuery("from " + tableName + " where FileDate='" + Convert.ToDateTime(date.Replace("\"","")).ToString("yyyy-MM-dd") + "'")
                           .SetFirstResult((currentPage - 1) * pageSize)
                           .SetMaxResults(pageSize)
                           .List<dynamic>();

                recordNumbers =
                    session.CreateQuery(" select count(*) from " + tableName)
                           .UniqueResult()
                           .ToString();

                tx.Commit();

                return dataList;
            }
        }

        private IList<Columns> getColumns(IEnumerable<PropertyInfo> propertyInfo)
        {
            return propertyInfo.Select(t => new Columns
            {
                field = t.Name,
                type = getTypes.getJSType(t),
                //aggLabelFilter = "",
                cellClass = "",
                cellFilter = "",
                cellTemplate = "",
                displayName = t.Name,
                editableCellTemplate = "",
                enableCellEdit = false,
                groupable = true,
                headerCellTemplate = "",
                headerClass = "",
                pinnable = true,
                resizable = true,
                sortable = true,
                width = "250",
                visible = true
            }).Where(t => !t.field.EndsWith("Id") && t.field != "Version" && t.field != "CreatedBy" && t.field != "CreatedOn" && t.field != "CreateAction" && t.field != "FileScheduler" && t.field != "Status" && t.field != "ApprovedBy" && t.field != "ApprovedOn").ToList();
        }

        
    }
}
