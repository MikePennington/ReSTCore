using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ReSTCore.Attributes;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.Routing;
using TestMvcApp.DTOs;

namespace TestMvcApp.Controllers
{
    [Help("This is just a test service")]
    public class ThingsController : RestController
    {
        private static Dictionary<int, Thing> _repo;

        public ThingsController()
        {
            if (_repo == null)
            {
                _repo = new Dictionary<int, Thing>
                            {
                                {1, new Thing {Name = "Thing 1"}},
                                {2, new Thing {Name = "Thing 2"}},
                                {3, new Thing {Name = "Thing 3"}}
                            };
            }
        }

        [Help("Lists things", Output = typeof(List<Thing>))]
        [HelpParam("param1", "better")]
        [HelpParam("param2", "faster")]
        [HelpParam("param3", "harder")]
        [HelpParam("param4", "stronger")]
        public ActionResult Index()
        {
            var things = _repo.Values.ToList();
            var result = new Result<List<Thing>>
            {
                Entity = things,
                ResultType = ResultType.Success
            };
            return null;
        }

    }
}
