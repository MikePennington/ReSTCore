using System;
using System.Web.Mvc;
using System.Web.Routing;
using ReSTCore.Routing;
using ReSTCore.ValueProviderFactories;

namespace ReSTCore
{
    public static class RestCore
    {
        public static string ServiceName { get; private set; }
        public static Uri ServiceBaseUri { get; private set; }

        public static void Register(string serviceName, string serviceBaseUri)
        {
            ServiceName = serviceName;

            if (!string.IsNullOrWhiteSpace(serviceBaseUri))
                ServiceBaseUri = new Uri(serviceBaseUri);

            // Remove MVC header
            MvcHandler.DisableMvcResponseHeader = true;

            // Register value provider factories
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());
        }

        public static void RegisterAllRoutes(string idRegex)
        {
            RestfulRouteHandler.BuildRoutes(RouteTable.Routes, idRegex);

            RouteTable.Routes.MapRoute(
                "HelpIndex",
                "help",
                new {controller = "Help", action = "Index"}
                );

            RouteTable.Routes.MapRoute(
                "DTO",
                "help/dtos/{dtoName}",
                new {controller = "Help", action = "DTO"}
                );

            RouteTable.Routes.MapRoute("DefaultPage", "", new {controller = "Help", action = "Index"});
        }
    }
}
