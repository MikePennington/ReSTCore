using System;
using System.Web.Mvc;
using System.Web.Routing;
using ReSTCore.Routing;
using ReSTCore.ValueProviderFactories;

namespace ReSTCore
{
    public static class RestCore
    {
        public static Configuration Configuration { get; set; }

        public static void Register(Configuration configuration)
        {
            Configuration = configuration;

            // Remove MVC header
            MvcHandler.DisableMvcResponseHeader = true;

            // Register value provider factories
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());

            // Register help routes
            RouteTable.Routes.MapRoute(
                "HelpIndex",
                "help",
                new {controller = "Help", action = "Index"}
                );

            RouteTable.Routes.MapRoute(
                "DTO",
                "help/dtos",
                new {controller = "Help", action = "DTO"}
                );

            RouteTable.Routes.MapRoute("DefaultPage", "", new {controller = "Help", action = "Index"});
        }

        /// <summary>
        /// Registers a ReST service by its controller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="idRegex"></param>
        public static void RegisterControllerRoutes(string controllerName, string idRegex)
        {
            RestfulRouteHandler.BuildRoutes(RouteTable.Routes, controllerName, idRegex, controllerName);
        }
    }
}
