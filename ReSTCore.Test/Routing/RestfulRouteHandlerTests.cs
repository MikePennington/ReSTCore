using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReSTCore.Routing;
using Should;

namespace ReSTCore.Test.Routing
{
    [TestClass]
    public class RestfulRouteHandlerTests
    {
        private const string ControllerName = "Tests";
        private RouteCollection _routes;

        [TestInitialize]
        public void TestInitialize()
        {
            _routes = new RouteCollection();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _routes = null;
        }

        [TestMethod]
        public void TestHelpRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/help", "GET").Defaults["Action"].ShouldEqual("Help");
        }

        [TestMethod]
        public void TestIndexRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName, "GET").Defaults["Action"].ShouldEqual("Index");
        }

        [TestMethod]
        public void TestShowRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}", "GET").Defaults["Action"].ShouldEqual("Show");
        }

        [TestMethod]
        public void TestShowPropertyRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}/{property}", "GET").Defaults["Action"].ShouldEqual("ShowProperty");
        }

        [TestMethod]
        public void TestCreateRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName, "POST").Defaults["Action"].ShouldEqual("Create");
        }

        [TestMethod]
        public void TestUpdateRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}", "PUT").Defaults["Action"].ShouldEqual("Update");
        }

        [TestMethod]
        public void TestUpdateViaGetRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}/put", "POST").Defaults["Action"].ShouldEqual("Update");
        }

        [TestMethod]
        public void TestUpdatePropertyRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}/{property}/{value}", "PUT").Defaults["Action"].ShouldEqual("UpdateProperty");
        }

        [TestMethod]
        public void TestDeleteRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}", "DELETE").Defaults["Action"].ShouldEqual("Delete");
        }

        [TestMethod]
        public void TestDeleteViaGetRoute()
        {
            RestfulRouteHandler.BuildRoutes(_routes, ControllerName, RegexPattern.MatchPositiveInteger, ControllerName);

            FindRoute(_routes, ControllerName + "/{id}/delete", "GET").Defaults["Action"].ShouldEqual("Delete");
        }

        private static Route FindRoute(RouteCollection routes, string uri, string method)
        {
            foreach (Route route in routes)
            {
                if (route.Url.Equals(uri, StringComparison.OrdinalIgnoreCase)
                    && ((HttpMethodConstraint)route.Constraints["httpMethod"]).AllowedMethods.Contains(method))
                    return route;
            }
            return null;
        }
    }
}
