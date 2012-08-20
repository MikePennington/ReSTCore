using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReSTCore.Models;
using ReSTCore.ResponseFormatting;
using ReSTCore.Test.Fixtures;
using Should;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.Routing;

namespace ReSTCore.Test.Controllers
{
    [TestClass]
    public class BaseControllerTests
    {
        private TestController _controller;

        [TestInitialize]
        public void TestInitialize()
        {

        }

        [TestCleanup]
        public void TestCleanup()
        {
            _controller = null;
        }

        [TestMethod]
        public void JsonAcceptTypeShouldReturnJson()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] {"application/json"}).Build();

            ActionResult result = _controller.Index();

            result.ShouldBeType<ActionResults.JsonResult>();
        }

        [TestMethod]
        public void JsonpAcceptTypeShouldReturnJsonp()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { "text/javascript" }).Build(); ;

            ActionResult result = _controller.Index();

            result.ShouldBeType<ActionResults.JsonpResult>();
        }

        [TestMethod]
        public void XmlAcceptTypeShouldReturnXml()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { "text/xml" }).Build(); ;

            ActionResult result = _controller.Index();

            result.ShouldBeType<ActionResults.XmlResult>();
        }

        [TestMethod]
        public void HtmlAcceptTypeShouldReturnHtml()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { "text/html" }).Build(); ;

            ActionResult result = _controller.Index();

            result.ShouldBeType<ViewResult>();
        }

        [TestMethod]
        public void SettingCustomMimeTypeMappingShouldReturnCorrectly()
        {
            const string acceptType = "custom";
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { acceptType }).Build(); ;
            ResponseMappingSettings.Settings = new ResponseMappingSettings
                    {
                        ResponseTypeMappings = new List<ResponseTypeMapping>
                                                    {
                                                        new ResponseTypeMapping(acceptType, ResponseFormatType.Jsonp)
                                                    }
                    };

            ActionResult result = _controller.Index();

            result.ShouldBeType<ActionResults.JsonpResult>();
        }

        [TestMethod]
        public void MultipleAcceptTypesShouldTakeFirstMatching()
        {
            _controller = TestControllerBuilder.TestController()
                .WithAcceptTypes(new[] { "text/unknown", "text/javascript", "text/html" }).Build(); ;

            ActionResult result = _controller.Index();

            result.ShouldBeType<ViewResult>();
        }
    }
}
