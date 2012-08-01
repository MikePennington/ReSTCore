using System.Web.Mvc;
using System.Web.Routing;
using ReSTCore.Routing;
using ReSTCore.ValueProviderFactories;

namespace ReSTCore
{
    public static class RestCore
    {
        public static void Register()
        {
            // Remove MVC header
            MvcHandler.DisableMvcResponseHeader = true;

            // Register value provider factories
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());
        }

        public static void RegisterAllRoutes(string idRegex)
        {
            RestfulRouteHandler.BuildRoutes(RouteTable.Routes, idRegex);
        }
    }
}
