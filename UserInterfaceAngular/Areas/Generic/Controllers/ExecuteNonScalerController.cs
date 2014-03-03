using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.Generic.Models;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{
    public class ExecuteNonScalerController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult ExecuteNonScalerSql()
        {
            var result = new QueryResult();
            return View(result);
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult ExecuteNonScalerSql(FormCollection value)
        {
            var query = value["Query"];
            var result = new QueryResult();
            var dataTable = QueryExecuter.ExecuteNonScaler(query);
            if (dataTable == null || dataTable.Rows.Count>-1)
            {
                result.Error = "Please check sql query or data not available in database";
            }
            result.Data = dataTable;
            result.AddQuery(query);
            return View(result);
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult ExecuteUpdateDelete()
        {
            var ressult = new QueryResult();
            return View(ressult);
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult ExecuteUpdateDelete(FormCollection value)
        {
            var query = value["Query"];
            var result = new QueryResult {RowsAffected = QueryExecuter.ExecuteNonQueryUpdateDelete(query)};
            result.AddQuery(query);
            return View(result);
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult ExecuteScalerWithSelect()
        {
            var result = new QueryResult();
            return View(result);
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult ExecuteScalerWithSelect(FormCollection value)
        {
            var result = new QueryResult();
            var query = value["Query"];
            result.AddQuery(query);
            result.RowsAffected = QueryExecuter.ExecuteScalerSelect(query);
            return View(result);
        }
    }
}
