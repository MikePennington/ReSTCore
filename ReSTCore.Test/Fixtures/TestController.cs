using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.Routing;

namespace ReSTCore.Test.Fixtures
{
    public class TestController : BaseController<int, TestDTO>
    {
        public override ActionResult Index()
        {
            var result = new Result<List<TestDTO>>
                             {
                                 Entity = new List<TestDTO>
                                              {
                                                  new TestDTO {Name = "Test 1"},
                                                  new TestDTO {Name = "Test 2"}
                                              },
                                 ResultType = ResultType.Success
                             };
            return HandleResult(RestfulAction.Index, result);
        }

        public ActionResult ThrowException(string errorMessage)
        {
            throw new Exception(errorMessage);
        }
    }
}
