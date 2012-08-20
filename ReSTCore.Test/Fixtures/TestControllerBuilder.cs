using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Moq;

namespace ReSTCore.Test.Fixtures
{
    public class TestControllerBuilder : IBuilder<TestController>
    {
        private readonly TestController _controller;
        private readonly Mock<HttpRequestBase> _request;
        private readonly Mock<HttpResponseBase> _response;

        private TestControllerBuilder()
        {
            _request = new Mock<HttpRequestBase>();
            _request.Setup(x => x.AcceptTypes).Returns(new string[]{});
            _request.Setup(x => x.QueryString).Returns(new NameValueCollection());

            _response = new Mock<HttpResponseBase>();

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Request).Returns(_request.Object);
            httpContext.Setup(x => x.Response).Returns(_response.Object);

            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = httpContext.Object;

            _controller = new TestController();
            _controller.ControllerContext = controllerContext;
        }

        public static TestControllerBuilder TestController()
        {
            return new TestControllerBuilder();
        }

        public TestController Build()
        {
            return _controller;
        }

        public TestControllerBuilder WithAcceptTypes(string[] acceptTypes)
        {
            _request.Setup(x => x.AcceptTypes).Returns(acceptTypes);
            return this;
        }
    }
}
