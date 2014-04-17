using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using AngularUI.appCode;
using AngularUI.Shared.webapis;
using ColloSys.UserInterface.App_Start;

namespace AngularUI
{
    public class WebApiApplication : System.Web.HttpApplication
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
            GlobalConfiguration.Configuration.Filters.Add(new SessionPerRequest());

            RegisterWebApiConfig(GlobalConfiguration.Configuration);

            JsonNetResult.InitSettings(GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
        }

        private static void RegisterWebApiConfig(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("DefaultApi",
                                       "api/{controller}/{action}/{id}",
                                       new { id = RouteParameter.Optional, id2 = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute("DefaultApi1",
                                      "api/{controller}/{id}",
                                      new { id = RouteParameter.Optional }
               );

            //GlobalConfiguration.Configuration.Filters.Add(new AuthorizeAttribute());

            // remove default xml formatter
            var appXmlType =
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}