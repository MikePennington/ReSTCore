using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReSTCore.Util;

namespace ReSTCore.Routing
{
    /// <summary>A Rails inspired Restful Routing Handler</summary>
    public class RestfulRouteHandler : MvcRouteHandler
    {
        private IRestfulActionResolver _actionResolver;

        internal RestfulRouteHandler()
        {
        }

        internal RestfulRouteHandler(IRestfulActionResolver actionResolver)
        {
            _actionResolver = actionResolver;
        }

        /// <summary>
        /// Adds the set of SimplyRestful routes to the <paramref name="routeCollection"/>.
        /// By default a positive integer validator is set for the Id parameter of the <see cref="Route.Values"/>.
        /// </summary>
        /// <param name="routeCollection">The route collection to add the routes to.</param>
        /// <param name="idRegex">The regex used to match the entity id</param>
        /// <seealso cref="BuildRoutes(RouteCollection,string,string,string)"/>
        public static void BuildRoutes(RouteCollection routeCollection, string idRegex)
        {
            BuildRoutes(routeCollection, "{controller}", idRegex, null);
        }

        /// <summary>
        /// Adds the set of SimplyRestful routes to the <paramref name="routeCollection"/>.
        /// By default a positive integer validator is set for the Id parameter of the <see cref="Route.Values"/>.
        /// </summary>
        /// <param name="routeCollection">The route collection to add the routes to.</param>
        /// <param name="areaPrefix">An area inside the site to prefix the <see cref="Route.Url"/> with.</param>
        /// <param name="idRegex">The regex used to match the entity id</param>
        /// <seealso cref="BuildRoutes(RouteCollection,string,string,string)"/>
        /// <example lang="c#">
        /// RestfulRouteHandler.BuildRoutes(RouteTable.Routes, "private/admin")
        /// // Generates the Urls private/admin/[controller]/new, private/admin/[controller]/[id]/edit, etc.
        /// </example>
        public static void BuildRoutes(RouteCollection routeCollection, string areaPrefix, string idRegex)
        {
            BuildRoutes(routeCollection, FixPath(areaPrefix) + "/{controller}", idRegex, null);
        }

        /// <summary>
        /// Adds the set of SimplyRestful routes to the <paramref name="routeCollection"/>.
        /// </summary>
        /// <param name="routeCollection">The route collection to add the  routes to.</param>
        /// <param name="controllerPath">The path to the controller, you can use the special matching [controller]</param>
        /// <param name="idValidationRegex">The <see cref="System.Text.RegularExpressions.Regex"/> 
        /// validator to add to the Id parameter of the <see cref="Route.Values"/>, use <c>null</c> to not validate the id.</param>
        /// <param name="controller">The name of the controller.  Only required if you are trying to route to a specific controller using a non-standard url.</param>
        public static void BuildRoutes(RouteCollection routeCollection, string controllerPath, string idValidationRegex, string controller)
        {
            controllerPath = FixPath(controllerPath);

            idValidationRegex = string.IsNullOrWhiteSpace(idValidationRegex) ? RegexPattern.MatchAny : idValidationRegex;

            // Help
            routeCollection.Add(new Route(
                controllerPath + "/help",
                BuildDefaults(Constants.Actions.Help, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new MvcRouteHandler()));

            // Show
            routeCollection.Add(new Route(
                controllerPath + "/{id}",
                BuildDefaults(Constants.Actions.Show, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET"), id = idValidationRegex }),
                new MvcRouteHandler()));
            routeCollection.Add(new Route(
                controllerPath + "/{id}/{property}",
                BuildDefaults(Constants.Actions.ShowProperty, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET"), id = idValidationRegex }),
                new MvcRouteHandler()));

            // Update
            routeCollection.Add(new Route(
                controllerPath + "/{id}",
                BuildDefaults(Constants.Actions.Update, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("PUT"), id = idValidationRegex }),
                new MvcRouteHandler()));
            routeCollection.Add(new Route(
                controllerPath + "/{id}/put",
                BuildDefaults(Constants.Actions.Update, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("POST"), id = idValidationRegex }),
                new MvcRouteHandler()));
            routeCollection.Add(new Route(
                controllerPath + "/{id}/{property}/{value}",
                BuildDefaults(Constants.Actions.UpdateProperty, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("PUT"), id = idValidationRegex }),
                new MvcRouteHandler()));

            // Delete
            routeCollection.Add(new Route(
                controllerPath + "/{id}",
                BuildDefaults(Constants.Actions.Delete, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("DELETE"), id = idValidationRegex }),
                new MvcRouteHandler()));
            routeCollection.Add(new Route(
                controllerPath + "/{id}/delete",
                BuildDefaults(Constants.Actions.Delete, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET"), id = idValidationRegex }),
                new MvcRouteHandler()));

            // Index
            routeCollection.Add(new Route(
                controllerPath,
                BuildDefaults(Constants.Actions.Index, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new MvcRouteHandler()));

            // Create
            routeCollection.Add(new Route(
                controllerPath,
                BuildDefaults(Constants.Actions.Create, controller),
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("POST") }),
                new MvcRouteHandler()));
        }

        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            EnsureActionResolver(requestContext.HttpContext);

            RestfulAction action = _actionResolver.ResolveAction(requestContext);
            if (action != RestfulAction.None)
            {
                requestContext.RouteData.Values["action"] = action.ToString();
            }
            return base.GetHttpHandler(requestContext);
        }

        /// <summary>Fixes an area prefix for the route url.</summary>
        /// <param name="path">The area prefix to fix.</param>
        /// <returns>A non null string with leading and trailing /'s stripped</returns>
        private static string FixPath(string path)
        {
            if (path == null)
            {
                return string.Empty;
            }
            return path.Trim().Trim(new[] { '/' });
        }

        /// <summary>Builds a Default object for a route.</summary>
        /// <param name="action">The default action for the route.</param>
        /// <param name="controllerName">The default controller for the route.</param>
        /// <returns>An Anonymous Type with a default Action property and and default Controller property
        /// if <paramref name="controllerName"/> is not null or empty.</returns>
        private static RouteValueDictionary BuildDefaults(string action, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
                return new RouteValueDictionary(new { Action = action });

            return
                new RouteValueDictionary(new { Action = action, Controller = controllerName });
        }

        /// <summary>Ensures that a <see cref="IRestfulActionResolver"/> exists.</summary>
        /// <param name="serviceProvider">The <see cref="HttpContextBase"/> as an <see cref="IServiceProvider"/> to try and use to resolve an instance of the <see cref="IRestfulActionResolver"/></param>
        /// <remarks>If no <see cref="IRestfulActionResolver"/> can be resolved the default <see cref="RestfulActionResolver"/> is used.</remarks>
        private void EnsureActionResolver(IServiceProvider serviceProvider)
        {
            if (_actionResolver == null)
            {
                _actionResolver = (IRestfulActionResolver)serviceProvider.GetService(typeof(IRestfulActionResolver));
                if (_actionResolver == null)
                    _actionResolver = new RestfulActionResolver();
            }
        }
    }
}
