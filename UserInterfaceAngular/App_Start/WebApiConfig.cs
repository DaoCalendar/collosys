using System.Linq;
using System.Web.Http;

namespace ColloSys.UserInterface.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("DefaultApi",
                                       "api/{controller}/{action}/{id}",
                                       new {id = RouteParameter.Optional, id2 = RouteParameter.Optional}
                );

            GlobalConfiguration.Configuration.Filters.Add(new AuthorizeAttribute());

            // remove default xml formatter
            var appXmlType =
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}