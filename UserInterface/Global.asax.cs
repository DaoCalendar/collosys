using System.Web.Http;

namespace Piotr.ProductList
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RegisterWebApiConfig(GlobalConfiguration.Configuration);
        }

        private static void RegisterWebApiConfig(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}