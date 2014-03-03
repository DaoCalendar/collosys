#region references

using System.Globalization;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using UserInterfaceAngular;
using System.Web;
using ColloSys.UserInterface.App_Start;
using UserInterfaceAngular.Shared;

#endregion

namespace ColloSys.UserInterface
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // disable [Required] error
            GlobalConfiguration.Configuration.Services.RemoveAll(
                typeof(System.Web.Http.Validation.ModelValidatorProvider),
                v => v is InvalidModelValidatorProvider);

            NLogConfig.InitConFig();
            NHibernateConfig.InitNHibernate();
            NHibernateConfig.ApplyDatabaseChanges();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            JsonNetResult.InitSettings(GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);

            //disable username check
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
        }

        protected void Session_End()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            Session.Abandon();
        }
    }
}


//private static int _initCount;
//protected void Application_BeginRequest(object sender, EventArgs e)
//{
//    if (_initCount++ != 0) return;
//    SessionManager.BindNewSession();
//}

//protected void Application_EndRequest(
//    object sender, EventArgs e)
//{
//    if (--_initCount == 0)
//    {
//        SessionManager.UnbindSession();
//    }
//}


//protected void Application_Error()
//{
//    try
//    {
//        var exception = Server.GetLastError();
//        Response.Clear();
//        Response.TrySkipIisCustomErrors = true;
//        Server.ClearError();
//        var routeData = new RouteData();
//        routeData.Values["controller"] = "Error";
//        routeData.Values["action"] = "Index";
//        routeData.Values["exception"] = exception;

//        IController errorsController = new ErrorController();
//        var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
//        errorsController.Execute(rc);
//    }
//    catch (Exception)
//    {
//        Response.TransmitFile("~/Views/Shared/Error.cshtml");
//    }
//}

