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

                var help = (HelpAttribute) Attribute.GetCustomAttribute(controllerType, typeof (HelpAttribute));
                if (help != null)
                {
                    if (help.Ignore)
                    {
                        HelpDisabled = true;
                        return;
                    }
                    ServiceDescription = help.Text;
                }

                var routeModel = new RouteModel(route, controller, action.ToString());
                if (!routeModel.Ignore)
                    Routes.Add(routeModel);
            }

            Routes.Sort();
        }
    }

    public class RouteModel : IComparable<RouteModel>
    {
        internal RouteModel(Route route, Controller controller, string action)
        {
            Order = int.MaxValue;
            Populate(route, controller, action);
        }

        public string Path { get; private set; }

        public string HttpVerb { get; private set; }

        public string MethodName { get; private set; }

        public string Description { get; private set; }

        public bool Ignore { get; private set; }

        public int Order { get; private set; }

        public List<ParameterModel> Parameters { get; private set; }

        private void Populate(Route route, Controller controller, string action)
        {
            Path = "/" + route.Url;
            Path = Path.Replace("{controller}", controller.RouteData.Values["controller"].ToString());

            // Get help information
            Type controllerType = controller.GetType();
            MethodInfo methodInfo = controllerType.GetMethod(action);
            if (methodInfo != null)
            {
                var help = (HelpAttribute) Attribute.GetCustomAttribute(methodInfo, typeof (HelpAttribute), false);
                if (help != null)
                {
                    if (help.Ignore)
                    {
                        Ignore = true;
                        return;
                    }
                    Description = help.Text;
                    Order = help.Order;
                }

                var helpParams = (HelpParamAttribute[])Attribute.GetCustomAttributes(methodInfo, typeof(HelpParamAttribute), false);
                Parameters = new List<ParameterModel>();
                foreach (var helpParam in helpParams)
                {
                    var paramModel = new ParameterModel
                    {
                        Name = helpParam.Name,
                        Description = helpParam.Text,
                        Order = helpParam.Order
                    };
                    Parameters.Add(paramModel);
                }
                Parameters.Sort();
            }

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

            MethodName = action;
        }

        public int CompareTo(RouteModel other)
        {
            int compare = Order.CompareTo(other.Order);
            if (compare != 0)
                return compare;

            compare = string.Compare(Path, other.Path, StringComparison.OrdinalIgnoreCase);
            if (compare != 0)
                return compare;
            return string.Compare(HttpVerb, other.HttpVerb, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class ParameterModel : IComparable<ParameterModel>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public int CompareTo(ParameterModel other)
        {
            int compare = Order.CompareTo(other.Order);
            if (compare != 0)
                return compare;

            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
