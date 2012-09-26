using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReSTCore;
using ReSTCore.Routing;

namespace TestMvcApp
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            // Put custom routes first
            routes.MapRoute(
                "GetSomething",
                "simple/getsomething",
                new {controller = "Simple", action = "GetSomething"},
                new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapRoute(
                "GetSomethingJson",
                "simple/getsomethingjson",
                new {controller = "Simple", action = "GetSomethingJson"}
                );

            routes.MapRoute(
                "DoSomething",
                "simple/dosomething",
                new {controller = "Simple", action = "DoSomething"},
                new {httpMethod = new HttpMethodConstraint("POST")}
                );

            var config = new Configuration {ServiceName = "Test Service"};
            RestCore.Register(config);
            RestCore.RegisterController("Things", RegexPattern.MatchPositiveInteger);
            RestCore.RegisterController("Simple", RegexPattern.MatchAny);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = ""} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}