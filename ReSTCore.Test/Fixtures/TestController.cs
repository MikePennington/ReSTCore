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
    public class TestController : BaseController<int, TestEntity>
    {
        public override ActionResult Index()
        {
            var result = new Result<List<TestEntity>>
            {
                Entity = new List<TestEntity>
                                                  {
                                                      new TestEntity {Name = "Test 1"},
                                                      new TestEntity {Name = "Test 2"}
                                                  },
                ResultType = ResultType.Success
            };
            return HandleResult(RestfulAction.Index, result);
        }
    }
}
