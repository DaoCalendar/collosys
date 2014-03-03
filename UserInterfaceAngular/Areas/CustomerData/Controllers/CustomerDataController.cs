using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.CustomerData.Controllers
{
    public class CustomerDataController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {            
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult NewIndex()
        {
            return View();
        }       
    }
}

#region Comment Code
//public NgGridOptions GetNgGrid<T>() where T : SharedPayment, new()
//{
//    var payments = SessionManager.GetAutomicDao<T>().GetAll();

//    foreach (var payment in payments)
//    {
//        payment.FileScheduler = null;
//    }

//    var gridOptions = new NgGridOptions
//    {
//        data = payments.Select(JObject.FromObject)
//    };

//    var gridColumns = GetPropertyList(typeof(T))
//                           .Select(prop => new ColumnDef()
//                           {
//                               field = prop.Name,
//                               displayName = prop.Name,
//                           });

//    gridOptions.columnDefs.AddRange(gridColumns);

//    return gridOptions;
//}

//private static IEnumerable<PropertyInfo> GetPropertyList(Type p)
//{
//    var propertyList = p.GetProperties(BindingFlags.Instance | BindingFlags.Public);

//    return from property in propertyList
//           let type = property.PropertyType
//           where type.BaseType == null || type.BaseType != typeof(Entity)
//           where type.BaseType == null || type.BaseType.BaseType == null || type.BaseType.BaseType != typeof(Entity)
//           where !type.IsGenericType || type.GetGenericTypeDefinition() == typeof(Nullable<>)
//           where !typeof(Entity).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(c => c.Name).Contains(property.Name)
//           select property;
//}
#endregion