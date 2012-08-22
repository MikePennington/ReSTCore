using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReSTCore.DTO;
using JsonResult = ReSTCore.ActionResults.JsonResult;

namespace ReSTCore.Test.ActionResults
{
    [TestClass]
    public class JsonResultTests
    {
        private Mock<HttpContextBase> _httpContext;
        private Mock<HttpRequestBase> _request;
        private Mock<HttpResponseBase> _response;

        [TestInitialize]
        public void TestInitialize()
        {
            _request = new Mock<HttpRequestBase>();

            _response = new Mock<HttpResponseBase>();
            _response.Setup(x => x.ContentType).Returns(string.Empty);

            _httpContext = new Mock<HttpContextBase>();
            _httpContext.Setup(x => x.Request).Returns(_request.Object);
            _httpContext.Setup(x => x.Response).Returns(_response.Object);
        }

        [TestMethod]
        public void NullDataShouldNotThrowError()
        {
            var controllerContext = new ControllerContext {HttpContext = _httpContext.Object};
            var result = new JsonResult(null);
            result.ExecuteResult(controllerContext);
            // Passes if no exception is thrown
        }
    }
}
