using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImpulseReSTCore;
using ImpulseReSTCore.Attributes;
using ImpulseReSTCore.DTO;
using ImpulseReSTCore.Mapping;
using ImpulseReSTCore.Routing;
using TestMvcApp.DTOs;

namespace TestMvcApp.Controllers
{
    public class ThingsController : BaseController<string, Thing>
    {
        [Help("The search method")]
        public override ActionResult Index()
        {
            var things = new List<Thing>
                             {
                                 new Thing {Id = "1", Name = "Thing 1"},
                                 new Thing {Id = "2", Name = "Thing 2"},
                                 new Thing {Id = "3", Name = "Thing 3"}
                             };
            var result = new Result<List<Thing>>
                             {
                                 Entity = things,
                                 ResultType = ResultType.Success
                             };
            return HandleResult(RestfulAction.Index, result);
        }

        [Help("The create method")]
        public override ActionResult Create(Thing thing)
        {
            if (!ValidateCreate(thing))
                return null;

            var result = new Result<Thing>
                             {
                                 Entity = new Thing {Id = "1", Name = "Thing 1"},
                                 ResultType = ResultType.Success
                             };
            return HandleResult(RestfulAction.Create, result);
        }

        [Help("The update method")]
        public override ActionResult Update(string id, Thing thing)
        {
            if (!ValidateUpdate(id, thing))
                return null;

            var result = new Result<Thing>
            {
                Entity = new Thing { Id = "1", Name = "Thing 1" },
                ResultType = ResultType.Success
            };

            return HandleResult(RestfulAction.Update, result);
        }
    }
}
