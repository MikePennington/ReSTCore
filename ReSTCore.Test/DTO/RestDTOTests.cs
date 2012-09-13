using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTCore.DTO;
using Should;

namespace ReSTCore.Test.DTO
{
    [TestClass]
    public class RestDTOTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            RestCore.Configuration = new Configuration();
        }

        [TestMethod]
        public void UrlShouldBuildWhenIdIsSet()
        {
            var testDto = new TestDTO {Id = "123"};

            testDto.Uri.ShouldEqual("http://localhost/Test/123");
        }

        [TestMethod]
        public void BaseUriShouldBeUsedToBuildUriWhenIdIsSet()
        {
            RestCore.Configuration.ServiceBaseUri = new Uri("http://test.com");

            var testDto = new TestDTO {Id = "123"};

            testDto.Uri.ShouldEqual("http://test.com/Test/123");
        }

        [TestMethod]
        public void ExceptionShouldNotBeThrownWhenCOnfigurationIsNull()
        {
            RestCore.Configuration = null;

            new TestDTO { Id = "123" };

            // Passed if no exception is thrown
        }
    
        private class TestDTO : RestDTO<string>
        {
            public override string Path
            {
                get { return "Test"; }
            }
        }
    }
}
