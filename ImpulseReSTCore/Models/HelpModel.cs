using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ImpulseReSTCore.Attributes;

namespace ImpulseReSTCore.Models
{
    public class HelpModel
    {
        public HelpModel()
        {
            Routes = new List<RouteModel>();
        }

        public HelpModel(Controller controller)
        {
            Routes = new List<RouteModel>();
            Populate(controller);
        }

        public string ServiceName { get; private set; }

        public string ServiceDescription { get; private set; }

        public bool HelpDisabled { get; private set; }

        public List<RouteModel> Routes { get; private set; }

        private void Populate(Controller controller)
        {
            Type controllerType = controller.GetType();
            foreach (var routeBase in controller.Url.RouteCollection)
            {
                if (routeBase.GetType() != typeof (Route))
                    continue;

                var route = (Route) routeBase;

                object controllerName;
                if (route.Defaults.TryGetValue("Controller", out controllerName))
                    ServiceName = controllerName.ToString();

                object action;
                if (!route.Defaults.TryGetValue("Action", out action))
                    continue;

                var controllerHelp = (HelpAttribute) Attribute.GetCustomAttribute(controllerType, typeof (HelpAttribute));
                if (controllerHelp != null)
                {
                    if (controllerHelp.Ignore)
                    {
                        HelpDisabled = true;
                        return;
                    }
                    ServiceDescription = controllerHelp.Description;
                }

                var routeModel = new RouteModel(route, controllerType, action.ToString());
                if (!routeModel.Ignore)
                    Routes.Add(routeModel);
            }

            Routes.Sort();
        }
    }

    public class RouteModel : IComparable<RouteModel>
    {
        internal RouteModel(Route route, Type controllerType, string action)
        {
            Populate(route, controllerType, action);
        }

        public string Path { get; private set; }

        public string HttpVerb { get; private set; }

        public string MethodName { get; private set; }

        public string Description { get; private set; }

        public bool Ignore { get; private set; }

        private void Populate(Route route, Type controllerType, string action)
        {
            Path = "/" + route.Url;

            MethodInfo methodInfo = controllerType.GetMethod(action);
            var methodHelp = (HelpAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(HelpAttribute), false);
            if (methodHelp != null)
            {
                if (methodHelp.Ignore)
                {
                    Ignore = true;
                    return;
                }
                Description = methodHelp.Description;
            }
            MethodName = action;

            // Get Http verbs
            foreach (var constraint in route.Constraints)
            {
                if (constraint.Key == "httpMethod" && constraint.Value.GetType() == typeof(HttpMethodConstraint))
                {
                    var httpVerbs = new StringBuilder();
                    foreach (string verb in ((HttpMethodConstraint)constraint.Value).AllowedMethods)
                    {
                        if (httpVerbs.Length > 0)
                            httpVerbs.Append(",");
                        httpVerbs.Append(verb);
                    }
                    HttpVerb = httpVerbs.ToString();
                }
            }
        }

        public int CompareTo(RouteModel other)
        {
            int pathCompare = string.Compare(Path, other.Path, StringComparison.OrdinalIgnoreCase);
            if (pathCompare != 0)
                return pathCompare;
            return string.Compare(HttpVerb, other.HttpVerb, StringComparison.OrdinalIgnoreCase);
        }
    }
}
