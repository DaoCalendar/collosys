using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ColloSys.DataLayer.FileUploader;
using ColloSys.UserInterface.Areas.Reporting.ViewModels;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Domain;
using System.Web.Script.Serialization;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Areas.Reporting.ViewModels;
using System.Collections;
using System.Text;
using UserInterfaceAngular.Filters;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.UserInterface.Areas.Reporting.Controllers
{
    public class ExportController : Controller
    {
        //
        // GET: /Reporting/Export/

        private Hashtable _filters = new Hashtable();
        private Hashtable _colFilters = new Hashtable();
        [UserActivity(Activity= ColloSysEnums.Activities.Reporting)]
        [MvcTransaction]
        public void GetExcel(string name,string JSfilter,string colfilterstr, string columns)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<FilterClass> filter = json_serializer.Deserialize<List<FilterClass>>(JSfilter);
            List<ColFilter> colfilterClass = json_serializer.Deserialize<List<ColFilter>>(colfilterstr);
            string[] columnsArr = columns.Replace("\"", "").Replace("[", "").Replace("]", "").Split(',');
            ReportFilterWrapper RW = new ReportFilterWrapper();
            MemoryStream stream = null;// ExcelUtility.GetExcel<CPayment>(DataList);
            _filters = new Hashtable();
            _colFilters = new Hashtable();
            if (filter != null && filter.Count > 0) commonUtility.BuildHql(filter, _filters); //buildHQL(filter.ToArray());
            
            //if (colfilterClass != null && colfilterClass.Count > 0) commonUtility.BuildHqlColumns(colfilterClass, _colFilters);
            stream = ExportUtility.GetExcel(getDataList(commonUtility.getTableNames(name)), columnsArr, commonUtility.getTableNames(name));

            //switch (name.ToUpper())
            //{
            //    case "FILE DETAILS":
            //         stream = ExportUtility.GetExcel<FileDetail>(getDataList<FileDetail>("FileDetail"),columnsArr);
            //        break;
            //    case "FILE COLUMNS":
            //        stream = ExportUtility.GetExcel<FileColumn>(getDataList<FileColumn>("FileColumn"), columnsArr);
            //        break;
            //    case "STAKE_HIERARCHY":
            //        stream = ExportUtility.GetExcel<StakeHierarchy>(getDataList<StakeHierarchy>("StakeHierarchy"), columnsArr);
            //        break;
            //    case "CACS ACTIVITY":
            //        stream = ExportUtility.GetExcel<CacsActivity>(getDataList<CacsActivity>("CacsActivity"), columnsArr);
            //        break;
            //    case "CREDIT CARD LINER":
            //        stream = ExportUtility.GetExcel<CLiner>(getDataList<CLiner>("CLiner"), columnsArr);
            //        break;
            //    case "CREDIT CARD PAYMENT":
            //        stream = ExportUtility.GetExcel<CPayment>(getDataList<CPayment>("CPayment"), columnsArr);
            //        break;
            //    case "CREDIT CARD UNBILLED":
            //        stream = ExportUtility.GetExcel<CUnbilled>(getDataList<CUnbilled>("CUnbilled"), columnsArr);
            //        break;
            //    case "CREDIT CARD WRITEOFF":
            //        stream = ExportUtility.GetExcel<CWriteoff>(getDataList<CWriteoff>("CWriteoff"), columnsArr);
            //        break;
            //    case "RLS LINER":
            //        stream = ExportUtility.GetExcel<RLiner>(getDataList<RLiner>("RLiner"), columnsArr);
            //        break;
            //    case "RLS PAYMENT":
            //        stream = ExportUtility.GetExcel<RPayment>(getDataList<RPayment>("RPayment"), columnsArr);
            //        break;
            //    case "RLS WRITEOFF":
            //        stream = ExportUtility.GetExcel<RWriteoff>(getDataList<RWriteoff>("RWriteoff"), columnsArr);
            //        break;
            //    case "EBBS LINER":
            //        stream = ExportUtility.GetExcel<ELiner>(getDataList<ELiner>("ELiner"), columnsArr);
            //        break;
            //    case "EBBS PAYMENT":
            //        stream = ExportUtility.GetExcel<EPayment>(getDataList<EPayment>("EPayment"), columnsArr);
            //        break;
            //    case "EBBS WRITEOFF":
            //        stream = ExportUtility.GetExcel<EWriteoff>(getDataList<EWriteoff>("EWriteoff"), columnsArr);
            //        break;
            //}
            if (stream != null)
            {
                //var filename = "ExampleExcel.xlsx";
                var contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Clear();
                Response.ContentType = contenttype;
                Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xlsx");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
        [MvcTransaction]
        public void GetCSV(string name, string JSfilter,string colfilterstr, string columns)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<FilterClass> filter = json_serializer.Deserialize<List<FilterClass>>(JSfilter);
            List<ColFilter> colfilterClass = json_serializer.Deserialize<List<ColFilter>>(colfilterstr);
            string[] columnsArr = columns.Replace("\"", "").Replace("[", "").Replace("]", "").Split(',');
            ReportFilterWrapper RW = new ReportFilterWrapper();
            StringBuilder sb = new StringBuilder();
            _filters = new Hashtable();
            _colFilters = new Hashtable();
            if (filter != null && filter.Count > 0) commonUtility.BuildHql(filter, _filters); //buildHQL(filter.ToArray());

           // if (colfilterClass != null && colfilterClass.Count > 0) commonUtility.BuildHqlColumns(colfilterClass, _colFilters);
            ExportUtility.GetCSV(sb, getDataList(commonUtility.getTableNames(name)), columnsArr, commonUtility.getTableNames(name));
            //switch (name.ToUpper())
            //{
            //    case "FILE DETAILS":
            //        ExportUtility.GetCSV(sb, getDataList("FileDetail"),columnsArr,"FileDetail");
            //        break;
            //    case "FILE COLUMNS":
            //        ExportUtility.GetCSV<FileColumn>(sb, getDataList<FileColumn>("FileColumn"), columnsArr);
            //        break;
            //    case "STAKE_HIERARCHY":
            //        ExportUtility.GetCSV<StakeHierarchy>(sb, getDataList<StakeHierarchy>("StakeHierarchy"), columnsArr);
            //        break;
            //    case "CACS ACTIVITY":
            //        ExportUtility.GetCSV<CacsActivity>(sb, getDataList<CacsActivity>("CacsActivity"), columnsArr);
            //        break;
            //    case "CREDIT CARD LINER":
            //        ExportUtility.GetCSV<CLiner>(sb, getDataList<CLiner>("CLiner"), columnsArr);
            //        break;
            //    case "CREDIT CARD PAYMENT":
            //        ExportUtility.GetCSV<CPayment>(sb, getDataList<CPayment>("CPayment"), columnsArr);
            //        break;
            //    case "CREDIT CARD UNBILLED":
            //        ExportUtility.GetCSV<CUnbilled>(sb, getDataList<CUnbilled>("CUnbilled"), columnsArr);
            //        break;
            //    case "CREDIT CARD WRITEOFF":
            //        ExportUtility.GetCSV<CWriteoff>(sb, getDataList<CWriteoff>("CWriteoff"), columnsArr);
            //        break;
            //    case "RLS LINER":
            //        ExportUtility.GetCSV<RLiner>(sb, getDataList<RLiner>("RLiner"), columnsArr);
            //        break;
            //    case "RLS PAYMENT":
            //        ExportUtility.GetCSV<RPayment>(sb, getDataList<RPayment>("RPayment"), columnsArr);
            //        break;
            //    case "RLS WRITEOFF":
            //        ExportUtility.GetCSV<RWriteoff>(sb, getDataList<RWriteoff>("RWriteoff"), columnsArr);
            //        break;
            //    case "EBBS LINER":
            //        ExportUtility.GetCSV<ELiner>(sb, getDataList<ELiner>("ELiner"),columnsArr);
            //        break;
            //    case "EBBS PAYMENT":
            //        ExportUtility.GetCSV<EPayment>(sb, getDataList<EPayment>("EPayment"),columnsArr);
            //        break;
            //    case "EBBS WRITEOFF":
            //        ExportUtility.GetCSV<EWriteoff>(sb, getDataList<EWriteoff>("EWriteoff"), columnsArr);
            //        break;
            //}
            if (sb.ToString().Length > 10)
            {
                Response.ClearContent();
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", name + ".csv"));
                Response.ContentType = "application/text";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(sb.ToString());
                Response.End();
            }
        }

        
         [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
         [MvcTransaction]
         public void GetExcelDate(string name, string date,string columns)
         {
             ReportFilterWrapper RW = new ReportFilterWrapper();
             MemoryStream stream = null;
             string[] columnsArr = columns.Replace("\"", "").Replace("[", "").Replace("]", "").Split(',');
             stream = ExportUtility.GetExcel(getDataList(commonUtility.getTableNames(name), date), columnsArr, commonUtility.getTableNames(name));

             if (stream != null)
             {
                 //var filename = "ExampleExcel.xlsx";
                 var contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                 Response.Clear();
                 Response.ContentType = contenttype;
                 Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xlsx");
                 Response.Cache.SetCacheability(HttpCacheability.NoCache);
                 Response.BinaryWrite(stream.ToArray());
                 Response.End();
             } 
         }

         [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
         [MvcTransaction]
         public void GetCSVDate(string name, string date, string columns)
         {
             string[] columnsArr = columns.Replace("\"", "").Replace("[", "").Replace("]", "").Split(',');
             ReportFilterWrapper RW = new ReportFilterWrapper();
             StringBuilder sb = new StringBuilder();
             ExportUtility.GetCSV(sb, getDataList(commonUtility.getTableNames(name), date), columnsArr,commonUtility.getTableNames(name));
            if (sb.ToString().Length > 10)
             {
                 Response.ClearContent();
                 Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", name + ".csv"));
                 Response.ContentType = "application/text";
                 Response.Cache.SetCacheability(HttpCacheability.NoCache);
                 Response.Write(sb.ToString());
                 Response.End();
             }
         }

        private IList<dynamic> getDataList(string TableName)
        {
            var session = SessionManager.GetCurrentSession();
            string filter = commonUtility.BuildFilter(_filters);
            string colFilter = commonUtility.BuildFilter(_colFilters);
             if (colFilter != "" && filter != "") colFilter = colFilter.Replace("WHERE", "AND");

             IList<dynamic> DataList = session.CreateQuery("from " + TableName + filter + colFilter).List<dynamic>();
            return DataList;
        }

        private IList<dynamic> getDataList(string TableName, string date)
        {
            var session = SessionManager.GetCurrentSession();
            IList<dynamic> DataList = session.CreateQuery("from " + TableName + " where FileDate='" + Convert.ToDateTime(date.Replace("\"","")).ToString("yyyy-MM-dd") + "'").List<dynamic>();
            return DataList;
        }
        
    }
}
