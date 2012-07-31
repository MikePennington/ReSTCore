using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ImpulseReSTCore.Routing;
using ImpulseReSTCore.ValueProviderFactories;

namespace ImpulseReSTCore
{
    public static class RestCore
    {
        public static void Register(bool registerRoutes)
        {
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());

            if(registerRoutes)
                RestfulRouteHandler.BuildRoutes(RouteTable.Routes);
        }
    }
}
