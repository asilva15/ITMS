using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebClientED
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Survey",
                url: "{controller}/{action}/{id}/{id1}",
                defaults: new { controller = "Survey", action = "Index", id = UrlParameter.Optional, id1 = UrlParameter.Optional }
            );
        }
    }
}
