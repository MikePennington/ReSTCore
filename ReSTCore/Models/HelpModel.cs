using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReSTCore.Attributes;
using ReSTCore.Util;

namespace ReSTCore.Models
{
    public class HelpModel
    {
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

            var serviceHelp = (HelpAttribute)Attribute.GetCustomAttribute(controllerType, typeof(HelpAttribute));
            if (serviceHelp != null)
            {
                if (serviceHelp.Ignore)
                {
                    HelpDisabled = true;
                    return;
                }
                ServiceDescription = serviceHelp.Text;
            }

            foreach (var methodInfo in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (methodInfo.ReturnType != typeof(ActionResult) && !methodInfo.ReturnType.IsSubclassOf(typeof(ActionResult)))
                    continue;

                string thisControllerName = controllerType.Name.ToLower().Replace("controller", "");
                string thisActionName = methodInfo.Name.ToLower();
                foreach (var routeBase in controller.Url.RouteCollection)
                {
                    if (routeBase.GetType() != typeof (Route))
                        continue;

                    var route = (Route) routeBase;
                    
                    var controllerNamePair = route.Defaults.FirstOrDefault(x => x.Key.ToLower() == "controller");
                    if(controllerNamePair.Value == null)
                        continue;
                    var controllerName = controllerNamePair.Value.ToString().ToLower();
                    if (controllerName != "{controller}" && controllerName != thisControllerName)
                        continue;

                    var actionNamePair = route.Defaults.FirstOrDefault(x => x.Key.ToLower() == "action"); 
                    if(actionNamePair.Value == null)
                        continue;
                    string actionName = actionNamePair.Value.ToString().ToLower();
                    if (actionName != "{action}" && actionName != thisActionName)
                        continue;

                    var routeModel = new RouteModel { MethodName = methodInfo.Name };

                    string path = route.Url.ToLower();
                    path = path.Replace("{controller}", controllerName);
                    path = path.Replace("{action}", actionName);

                    var methodHelp = (HelpAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(HelpAttribute), false);
                    if (methodHelp != null)
                    {
                        if (methodHelp.Ignore)
                            continue;

                        routeModel.Description = methodHelp.Text;
                        routeModel.Order = methodHelp.Order;
                    }

                    // Get Http verbs
                    string httpVerb = "ANY";
                    if (Attribute.GetCustomAttribute(methodInfo, typeof(HttpGetAttribute), false) != null)
                        httpVerb = "GET";
                    else if (Attribute.GetCustomAttribute(methodInfo, typeof(HttpPostAttribute), false) != null)
                        httpVerb = "POST";
                    var httpMethodContraint = route.Constraints.FirstOrDefault(x => x.Key.ToLower() == "httpmethod");
                    if (httpMethodContraint.Value != null && httpMethodContraint.Value.GetType() == typeof(HttpMethodConstraint))
                    {
                        var httpVerbs = new StringBuilder();
                        foreach (string verb in ((HttpMethodConstraint) httpMethodContraint.Value).AllowedMethods)
                        {
                            if (httpVerbs.Length > 0)
                                httpVerbs.Append(",");
                            httpVerbs.Append(verb);
                        }
                        httpVerb = httpVerbs.ToString();
                    }

                    var pathInfo = new PathInfo {Path = path, HttpVerb = httpVerb};

                    // Parameters
                    var helpParams = (HelpParamAttribute[])Attribute.GetCustomAttributes(methodInfo, typeof(HelpParamAttribute), false);
                    routeModel.Parameters = new List<ParameterModel>();
                    foreach (var helpParam in helpParams)
                    {
                        var paramModel = new ParameterModel
                        {
                            Name = helpParam.Name,
                            Description = helpParam.Text,
                            Order = helpParam.Order
                        };
                        routeModel.Parameters.Add(paramModel);
                    }
                    routeModel.Parameters.Sort();

                    var existingRouteModel = Routes.FirstOrDefault(x => x.MethodName == routeModel.MethodName);
                    if (existingRouteModel != null)
                    {
                        existingRouteModel.PathInfo.Add(pathInfo);
                    }
                    else
                    {
                        routeModel.PathInfo.Add(pathInfo);
                        Routes.Add(routeModel);                     
                    }
                }
            }

            //foreach (var routeBase in controller.Url.RouteCollection)
            //{
            //    if (routeBase.GetType() != typeof (Route))
            //        continue;

            //    var route = (Route) routeBase;

            //    object controllerName;
            //    if (route.Defaults.TryGetValue("Controller", out controllerName))
            //        ServiceName = controllerName.ToString();
            //    if (!string.Equals(controller.GetType().Name, ServiceName + "Controller", StringComparison.OrdinalIgnoreCase))
            //        continue;
            //    if (string.IsNullOrWhiteSpace(route.Url) || route.Url.StartsWith("dtos") || route.Url.StartsWith("help") 
            //        || route.Url.EndsWith("help"))
            //        continue;

            //    object action;
            //    if (!route.Defaults.TryGetValue("Action", out action))
            //        continue;

            //    var help = (HelpAttribute) Attribute.GetCustomAttribute(controllerType, typeof (HelpAttribute));
            //    if (help != null)
            //    {
            //        if (help.Ignore)
            //        {
            //            HelpDisabled = true;
            //            return;
            //        }
            //        ServiceDescription = help.Text;
            //    }

            //    var routeModel = new RouteModel(route, controller, action.ToString());
            //    if (!routeModel.Ignore)
            //        Routes.Add(routeModel);
            //}

            Routes.Sort();
        }
    }

    public class RouteModel : IComparable<RouteModel>
    {
        internal RouteModel()
        {
            Order = int.MaxValue;
            PathInfo = new List<PathInfo>();
            Parameters = new List<ParameterModel>();
            //Populate(route, controller, action);
        }

        //internal RouteModel(Route route, Controller controller, string action)
        //{
        //    Order = int.MaxValue;
        //    Populate(route, controller, action);
        //}

        public string MethodName { get; internal set; }

        public string Description { get; internal set; }

        public bool Ignore { get; internal set; }

        public int Order { get; internal set; }

        public List<ParameterModel> Parameters { get; internal set; }

        public List<PathInfo> PathInfo { get; internal set; }

        //private void Populate(Route route, Controller controller, string action)
        //{
        //    Path = "/" + route.Url;
        //    Path = Path.Replace("{controller}", controller.RouteData.Values["controller"].ToString());

        //    // Get help information
        //    Type controllerType = controller.GetType();
        //    MethodInfo methodInfo = controllerType.GetMethod(action);
        //    if (methodInfo != null)
        //    {
        //        var help = (HelpAttribute) Attribute.GetCustomAttribute(methodInfo, typeof (HelpAttribute), false);
        //        if (help != null)
        //        {
        //            if (help.Ignore)
        //            {
        //                Ignore = true;
        //                return;
        //            }
        //            Description = help.Text;
        //            Order = help.Order;
        //        }

        //        var helpParams = (HelpParamAttribute[])Attribute.GetCustomAttributes(methodInfo, typeof(HelpParamAttribute), false);
        //        Parameters = new List<ParameterModel>();
        //        foreach (var helpParam in helpParams)
        //        {
        //            var paramModel = new ParameterModel
        //            {
        //                Name = helpParam.Name,
        //                Description = helpParam.Text,
        //                Order = helpParam.Order
        //            };
        //            Parameters.Add(paramModel);
        //        }
        //        Parameters.Sort();
        //    }

        //    // Get Http verbs
        //    foreach (var constraint in route.Constraints)
        //    {
        //        if (constraint.Key == "httpMethod" && constraint.Value.GetType() == typeof(HttpMethodConstraint))
        //        {
        //            var httpVerbs = new StringBuilder();
        //            foreach (string verb in ((HttpMethodConstraint)constraint.Value).AllowedMethods)
        //            {
        //                if (httpVerbs.Length > 0)
        //                    httpVerbs.Append(",");
        //                httpVerbs.Append(verb);
        //            }
        //            HttpVerb = httpVerbs.ToString();
        //        }
        //    }

        //    MethodName = action;
        //}

        public int CompareTo(RouteModel other)
        {
            int compare = Order.CompareTo(other.Order);
            if (compare != 0)
                return compare;

            return string.Compare(MethodName, other.MethodName, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class PathInfo
    {
        public string Path { get; internal set; }

        public string HttpVerb { get; internal set; }
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
