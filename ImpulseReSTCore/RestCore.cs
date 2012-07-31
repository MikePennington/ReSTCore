using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ImpulseReSTCore.ValueProviderFactories;

namespace ImpulseReSTCore
{
    public static class RestCore
    {
        public static void Register()
        {
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            System.Web.Mvc.ValueProviderFactories.Factories.Add(new XmlValueProviderFactory());
        }
    }
}
