﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.ResponseFormatting;
using ReSTCore.Test.Fixtures;
using ReSTCore.Util;
using Should;

namespace ReSTCore.Test.Controllers
{
    [TestClass]
    public class BaseControllerTests
    {
        private TestController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            RestCore.Configuration = new Configuration();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _controller = null;
            ResponseMappingSettings.ResetToDefaultSettings();
        }

        [TestMethod]
        public void JsonAcceptTypeShouldReturnJson()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] {"application/json"}).Build();

            ActionResult result = _controller.Index();

            result.ShouldBeType<ReSTCore.ActionResults.JsonResult>();
        }

        [TestMethod]
        public void JsonpAcceptTypeShouldReturnJsonp()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { "text/javascript" }).Build();

            ActionResult result = _controller.Index();

            result.ShouldBeType<ReSTCore.ActionResults.JsonpResult>();
        }

        [TestMethod]
        public void XmlAcceptTypeShouldReturnXml()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { "text/xml" }).Build();

            ActionResult result = _controller.Index();

            result.ShouldBeType<ReSTCore.ActionResults.XmlResult>();
        }

        [TestMethod]
        public void HtmlAcceptTypeShouldReturnHtml()
        {
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { "text/html" }).Build();

            ActionResult result = _controller.Index();

            result.ShouldBeType<ViewResult>();
        }

        [TestMethod]
        public void SettingCustomMimeTypeMappingShouldReturnCorrectly()
        {
            const string acceptType = "custom";
            _controller = TestControllerBuilder.TestController().WithAcceptTypes(new[] { acceptType }).Build();
            ResponseMappingSettings.Settings = new ResponseMappingSettings
                    {
                        ResponseTypeMappings = new List<ResponseTypeMapping>
                                                    {
                                                        new ResponseTypeMapping(acceptType, ResponseFormatType.Jsonp)
                                                    }
                    };

            ActionResult result = _controller.Index();

            result.ShouldBeType<ReSTCore.ActionResults.JsonpResult>();
        }

        [TestMethod]
        public void MultipleAcceptTypesShouldTakeFirstMatching()
        {
            _controller = TestControllerBuilder.TestController()
                .WithAcceptTypes(new[] { "text/unknown", "text/javascript", "text/html" }).Build();

            ActionResult result = _controller.Index();

            result.ShouldBeType<ReSTCore.ActionResults.JsonpResult>();
        }

        [TestMethod]
        public void ExceptionShouldBeWrittenToHeader()
        {
            const string error = "oh my, an exception!";

            _controller = TestControllerBuilder.TestController().Build();

            var context = new ExceptionContext { Exception = new Exception(error) };
            _controller.ThrowException(context);

            _controller.Headers.Get(Constants.Headers.ErrorMessage).ShouldEqual(error);
        }

        [TestMethod]
        public void ExceptionShouldBeWrittenToHeaderIfRealExceptionsAreTurnedOff()
        {
            const string realError = "real error";
            const string shownError = "shown error";

            _controller = TestControllerBuilder.TestController().Build();
            RestCore.Configuration = new Configuration {HideRealException = true, DefaultExceptionText = shownError};

            var context = new ExceptionContext { Exception = new Exception(realError) };
            _controller.ThrowException(context);

            _controller.Headers.Get(Constants.Headers.ErrorMessage).ShouldEqual(shownError);
        }

        [TestMethod]
        public void UnknownServerErrorCodeShouldBeWrittenToHeaderIfSet()
        {
            const int unknownServerErrorCode = 14;

            _controller = TestControllerBuilder.TestController().Build();
            RestCore.Configuration = new Configuration { UnknownServerErrorCode = unknownServerErrorCode };

            var context = new ExceptionContext { Exception = new Exception("error") };
            _controller.ThrowException(context);

            _controller.Headers.Get(Constants.Headers.ErrorCode).ShouldEqual(unknownServerErrorCode.ToString());
        }

        [TestMethod]
        public void ValicationErrorCodeShouldSetInvalidUserInputErrorCode()
        {
            const int invalidUserIntputErrorCode = 15;
            RestCore.Configuration = new Configuration { InvalidUserIntputErrorCode = invalidUserIntputErrorCode };
            var testDTO = new TestDTO { Name = "1234567890123456789012345678901234567890" };

            _controller = TestControllerBuilder.TestController().Build();

            _controller.Create(testDTO);

            _controller.Headers.Get(Constants.Headers.ErrorCode).ShouldEqual(invalidUserIntputErrorCode.ToString());
        }

        [TestMethod]
        public void CallWithNonMockedHttpContextShouldNotThrowException()
        {
            _controller = new TestController();
            _controller.Index();
        }
    }
}
