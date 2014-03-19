#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Areas.Reporting.ViewModels;
using ColloSys.UserInterface.Areas.Reporting.ViewModels;
using System.Collections;
using ColloSys.DataLayer.Enumerations;
using System.Web.Compilation;
//using ColloSys.UserInterface.Areas.Reporting.ViewModels;

#endregion

namespace ColloSys.UserInterface.Areas.Reporting.apiController
{
    public class ReportApiController : ApiController
    {
        private Hashtable _filters = new Hashtable();

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

         [HttpTransaction]
        public IEnumerable<dynamic> GetUnique(string aliasName, string columnName)
        {
            var session = SessionManager.GetCurrentSession();
            string tableName = string.Empty;
            string propertyName = string.Empty;
            IEnumerable<dynamic> dataList = null;
            dataList = GetUniqueDataList<FileDetail>(columnName, commonUtility.getTableNames(aliasName));
            return dataList;
        }

        [HttpTransaction]
        public IEnumerable<Reports> GetReportNames()
        {
   
             var session = SessionManager.GetCurrentSession();
             using (var tx = session.BeginTransaction())
             {
                 IEnumerable<Reports> dataList = session.QueryOver<Reports>().List();
                 tx.Commit();

                 return dataList;
             }
        }

        [HttpPost]
        [HttpTransaction]
        public ReportFilterWrapper GetFilter(FilterClass[] filter, string colfilter, int pageSize, int currentPage, string aliasName, string sortInfo)
        {
            var rw = new ReportFilterWrapper();
            _filters = new Hashtable();
            if (filter != null && filter.Length > 0) commonUtility.BuildHql(filter,_filters);
            bindDataList(commonUtility.getTableNames(aliasName), pageSize, currentPage, sortInfo, ref rw);
            return rw;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public string saveReport(FilterClass[] filter, string colfilter, string reportName, string aliasName, string columns)
        {
            string msg = string.Empty;

            if (!checkReportName(reportName))
            {
                return "Report name already exist, please enter different report name.";
            }

            if (filter != null && filter.Length > 0) commonUtility.BuildHql(filter,_filters);
            
            //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
             Reports rs = new Reports();
            rs.Name = reportName;
            rs.Filter = Newtonsoft.Json.JsonConvert.SerializeObject(filter);// json_serializer.Serialize(filter);// query;
            rs.TableName = aliasName;
            rs.Columns = columns;
            rs.ColumnsFilter = colfilter;
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(rs);
                tx.Commit();
            }
            return "Report saved successfully!";
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public string updateReport(Reports report, int updateFlag)
        {
          string msg = "1|Report updated successfully!";

          if (!checkReportName(report.Name) && updateFlag != 1)
          {
              return "0|Report name already exist, please enter different report name.";
          }
            
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(report);
                tx.Commit();
            }

            if (report.Version == 1 && updateFlag != 1) msg = "1| Same Report generated successfully.";
            return msg;
        }

        
        [HttpPost]
        [HttpTransaction(Persist = true)]
        public string deleteReport(Reports report)
        {
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                session.Delete(report);
                tx.Commit();
            }
            return "Report deleted successfully!";
        }
      
        
        #region private funtions
        private IEnumerable<dynamic> GetDataList(string tableName, int pageSize, int currentPage,string sort, out string recordNumbers)
        {
            var filter = commonUtility.BuildFilter(_filters);
            var session = SessionManager.GetCurrentSession();
            string orderby = string.Empty;
            if (sort.Split('|')[0] != "" && sort.Split('|')[1] != "")
            {
                orderby = " order by " + sort.Split('|')[0] + " " + sort.Split('|')[1];
            }


            using (var tx = session.BeginTransaction())
            {
                IEnumerable<dynamic> dataList = null;
                try
                {
                    dataList = session.CreateQuery("from " + tableName + filter + orderby) //  order by FileRowNo desc
                             .SetFirstResult((currentPage - 1) * pageSize)
                             .SetMaxResults(pageSize)
                             .List<dynamic>();
                }
                catch {
                    dataList = session.CreateQuery("from " + tableName + filter) //  IF column is Empty. order by statement throws error when there is empty column.
                                 .SetFirstResult((currentPage - 1) * pageSize)
                                 .SetMaxResults(pageSize)
                                 .List<dynamic>();
                }

                
                recordNumbers =
                    session.CreateQuery(" select count(*) from " + tableName + filter)
                           .UniqueResult()
                           .ToString();

                tx.Commit();

                return dataList;
            }
        }
        private bool checkReportName(string ReportName)
        {
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                string recordNumbers = session.CreateQuery(" select count(*) from Reports Where Name='" + ReportName + "'").UniqueResult().ToString();
                 tx.Commit();
                 if (Convert.ToInt16(recordNumbers) > 0)
                 {
                     return false;
                 }
                 else return true;
                
            }
        }
        private IList<Columns> getColumns(IEnumerable<PropertyInfo> propertyInfo)
        {
            return propertyInfo.Select(t => new Columns
                {
                    field = t.Name,
                    type = getTypes.getJSType(t),
                    aggLabelFilter = "",
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
        private IEnumerable<dynamic> GetUniqueDataList<T>(string columnName, string tableName)
        {
            IEnumerable<dynamic> dataList = null;
            var session = SessionManager.GetCurrentSession();
            if (commonUtility.GetType(tableName).GetProperty(columnName).PropertyType.UnderlyingSystemType.BaseType.Name.ToUpper() != "ENUM")
            {
                using (var tx = session.BeginTransaction())
                {
                    dataList = session.CreateQuery(" select distinct " + columnName + " from " + tableName).List<dynamic>().ToArray();
                    tx.Commit();
                }
            }

            else
            {
                switch (commonUtility.GetType(tableName).GetProperty(columnName).PropertyType.Name.ToUpper())
                {
                    case "PRODUCTS":
                        dataList = getEnumToStringArray<ScbEnums.Products>();
                        break;
                    case "CATEGORY":
                        dataList = getEnumToStringArray<ScbEnums.Category>();
                        break;
                    case "SCBSYSTEMS":
                        dataList = getEnumToStringArray<ScbEnums.ScbSystems>();
                        break;
                }
            }
            return dataList;
        }
        private IEnumerable<string> getEnumToStringArray<T>()
        {
            var values = Enum.GetValues(typeof(T));
            var result = new string[values.Length];
            int count = 0;
            foreach (var value in values)
            {
                result[count] = value.ToString();
                count++;
            }
            return result;
        }
        private void bindDataList(string className, int pageSize, int currentPage, string sortInfo, ref ReportFilterWrapper rw)
        {
            string value;
            rw.DataList = GetDataList(className, pageSize, currentPage, sortInfo, out value);
            rw.totalRecords = value;
            //rw.Columns = getColumns(commonUtility.GetType(className).GetProperties());
            IList<Columns> cols = getColumns(commonUtility.GetType(className).GetProperties());
            PropertyInfo[] columns = commonUtility.GetType(className).GetProperties();
            for (int _cols = 0; _cols < columns.Length; _cols++)
            {
                if (getTypes.GetCoreType(columns[_cols].PropertyType).Name.ToUpper() == "DATETIME")
                {
                    for (int _innerCols = 0; _innerCols < cols.Count; _innerCols++)
                    {
                        if (columns[_cols].Name.ToUpper() == cols[_innerCols].field.ToUpper())
                        {
                            cols[_innerCols].cellFilter = "date:'dd-MMM-yyyy'";
                            break;
                        }
                    }
                 }
            }

            rw.Columns = cols;
        }
   #endregion
    }
}
