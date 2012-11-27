using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
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
            Routes = new List<RouteInfo>();
            Populate(controller);
        }

        public string ServiceName { get; private set; }

        public string ServiceDescription { get; private set; }

        public bool HelpDisabled { get; private set; }

        public List<RouteInfo> Routes { get; private set; }

        private void Populate(Controller controller)
        {
            Type controllerType = controller.GetType();

            var serviceHelp = (HelpAttribute)Attribute.GetCustomAttribute(controllerType, typeof(HelpAttribute), false);
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
                foreach (var routeBase in controller.Url.RouteCollection)
                {
                    if (routeBase.GetType() != typeof (Route))
                        continue;

                    var route = (Route) routeBase;

                    string controllerName;
                    if(IsDefaultControllerRoute(route))
                    {
                        controllerName = thisControllerName;
                    }
                    else
                    {
                        controllerName = GetRouteValue<string>(route.Defaults, "controller");
                        if (controllerName != "{controller}" 
                            && !controllerName.Equals(thisControllerName, StringComparison.OrdinalIgnoreCase))
                            continue;                        
                    }

                    string actionName;
                    if(IsDefaultActionRoute(route))
                    {
                        actionName = methodInfo.Name;
                    }
                    else
                    {
                        actionName = GetRouteValue<string>(route.Defaults, "action");
                        if (actionName != "{action}" && !actionName.Equals(methodInfo.Name, StringComparison.OrdinalIgnoreCase))
                            continue;                        
                    }

                    ServiceName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(controllerName);

                    var routeInfo = new RouteInfo { MethodName = methodInfo.Name };

                    string path = route.Url.ToLower();
                    path = path.Replace("{controller}", controllerName);
                    path = path.Replace("{action}", actionName);
                    var parameters = methodInfo.GetParameters();
                    if(parameters.Length == 0 || parameters.FirstOrDefault(x => x.Name.ToLower() == "id") == null)
                        path = path.Replace("/{id}", string.Empty);

                    var methodHelp = (HelpAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(HelpAttribute), false);
                    if (methodHelp != null)
                    {
                        if (methodHelp.Ignore)
                            continue;

                        routeInfo.Description = methodHelp.Text;
                        routeInfo.Order = methodHelp.Order;
                        if (methodHelp.Input != null)
                            routeInfo.RequestDTO = BuildTypeName(methodHelp.Input);
                        if (methodHelp.Output != null)
                            routeInfo.ResponseDTO = BuildTypeName(methodHelp.Output);
                    }

                    // Get Http verbs
                    string httpVerb = "ANY";
                    if (Attribute.GetCustomAttribute(methodInfo, typeof(HttpGetAttribute), false) != null)
                        httpVerb = "GET";
                    else if (Attribute.GetCustomAttribute(methodInfo, typeof(HttpPostAttribute), false) != null)
                        httpVerb = "POST";
                    var httpMethodContraint = GetRouteValue<HttpMethodConstraint>(route.Constraints, "httpmethod");
                    if (httpMethodContraint != null)
                    {
                        var httpVerbs = new StringBuilder();
                        foreach (string verb in httpMethodContraint.AllowedMethods)
                        {
                            if (httpVerbs.Length > 0)
                                httpVerbs.Append(",");
                            httpVerbs.Append(verb);
                        }
                        httpVerb = httpVerbs.ToString();
                    }

                    var pathInfo = new PathInfo {Path = path.ToLower(), HttpVerb = httpVerb.ToUpper()};

                    // Parameters
                    var helpParams = (HelpParamAttribute[])Attribute.GetCustomAttributes(methodInfo, typeof(HelpParamAttribute), false);
                    routeInfo.Parameters = new List<ParameterInfo>();
                    foreach (var helpParam in helpParams)
                    {
                        var paramModel = new ParameterInfo
                        {
                            Name = helpParam.Name,
                            Description = helpParam.Text,
                            Order = helpParam.Order
                        };
                        routeInfo.Parameters.Add(paramModel);
                    }
                    routeInfo.Parameters.Sort();

                    var existingRouteModel = Routes.FirstOrDefault(x => x.MethodName == routeInfo.MethodName);
                    if (existingRouteModel != null)
                    {
                        if (!existingRouteModel.PathInfo.Contains(pathInfo))
                            existingRouteModel.PathInfo.Add(pathInfo);
                    }
                    else
                    {
                        routeInfo.PathInfo.Add(pathInfo);
                        Routes.Add(routeInfo);                     
                    }
                }
            }

            Routes.Sort();
        }

        private bool IsDefaultControllerRoute(Route route)
        {
            string[] uriParts = route.Url.Split('/');
            if (uriParts.Length < 1 || string.IsNullOrWhiteSpace(uriParts[0]))
                return false;
            return uriParts[0].Equals("{controller}", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsDefaultActionRoute(Route route)
        {
            string[] uriParts = route.Url.Split('/');
            if (uriParts.Length < 2 || string.IsNullOrWhiteSpace(uriParts[1]))
                return false;
            return uriParts[1].Equals("{action}", StringComparison.OrdinalIgnoreCase);
        }

        private T GetRouteValue<T>(IEnumerable<KeyValuePair<string, object>> routeValues, string key) where T : class
        {
            var pair = routeValues.FirstOrDefault(x => x.Key.ToLower() == key);
            return pair.Value as T;
        }

        private static string BuildTypeName(Type type)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return type.Name;
            }
            var genericArguments = type.GetGenericArguments();
            var typeDefeninition = type.Name;
            var unmangledName = typeDefeninition.Substring(0, typeDefeninition.IndexOf("`"));
            return unmangledName + "<" + String.Join(",", genericArguments.Select(BuildTypeName)) + ">";
        }
    }

    public class RouteInfo : IComparable<RouteInfo>
    {
        internal RouteInfo()
        {
            Order = int.MaxValue;
            PathInfo = new List<PathInfo>();
            Parameters = new List<ParameterInfo>();
        }

        public string MethodName { get; internal set; }

        public string Description { get; internal set; }

        public bool Ignore { get; internal set; }

        public int Order { get; internal set; }

        public List<ParameterInfo> Parameters { get; internal set; }

        public string RequestDTO { get; internal set; }

        public string ResponseDTO { get; internal set; }

        public List<PathInfo> PathInfo { get; internal set; }

        public int CompareTo(RouteInfo other)
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
        
        public bool Equals(PathInfo other)
        {
            if (other == null || Path == null || HttpVerb == null)
                return false;
            return Path.Equals(other.Path, StringComparison.OrdinalIgnoreCase)
                && HttpVerb.Equals(other.HttpVerb, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (Path == null || HttpVerb == null)
                return false;
            return Path.Equals(((PathInfo)obj).Path, StringComparison.OrdinalIgnoreCase)
                && HttpVerb.Equals(((PathInfo)obj).HttpVerb, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Path == null ? 0 : Path.GetHashCode();
        }
    }

    public class ParameterInfo : IComparable<ParameterInfo>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public int CompareTo(ParameterInfo other)
        {
            int compare = Order.CompareTo(other.Order);
            if (compare != 0)
                return compare;

            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
