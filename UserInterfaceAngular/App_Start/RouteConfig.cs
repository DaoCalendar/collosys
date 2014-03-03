using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UserInterfaceAngular
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("favicon.ico");

            //routes.MapRoute(
            //    name: "collosys_default",
            //    url: "collosys/{controller}/{action}/{id}",
            //    defaults: new {controller = "Account", action = "Login",  id = UrlParameter.Optional}
            //    );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}