#region references

using System.Text;
using System.Web.Mvc;
using UserInterfaceAngular.Shared;

#endregion

namespace ColloSys.UserInterface.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class BaseController : Controller
    {
        protected override JsonResult Json(object data,
            // ReSharper disable OptionalParameterHierarchyMismatch
                                            string contentType = "application/json",
                                            Encoding contentEncoding = null,
                                            JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        // ReSharper restore OptionalParameterHierarchyMismatch
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = behavior
            };
        }
    }
}
