using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ReSTCore;
using ReSTCore.Attributes;
using ReSTCore.DTO;
using ReSTCore.Routing;
using TestMvcApp.DTOs;

namespace TestMvcApp.Controllers
{
    [Help("This is just a test service")]
    public class ThingsController : BaseController<int, Thing>
    {
        private static Dictionary<int, Thing> _repo;

        public ThingsController()
        {
            if (_repo == null)
            {
                _repo = new Dictionary<int, Thing>
                           {
                               {1, new Thing {Id = 1, Name = "Thing 1"}},
                               {2, new Thing {Id = 2, Name = "Thing 2"}},
                               {3, new Thing {Id = 3, Name = "Thing 3"}}
                           };
            }
        }

        [Help("Lists things")]
        public override ActionResult Index()
        {
            var things = _repo.Values.ToList();
            var result = new Result<List<Thing>>
                             {
                                 Entity = things,
                                 ResultType = ResultType.Success
                             };
            return HandleResult(RestfulAction.Index, result);
        }

        [Help("Creates a new thing")]
        public override ActionResult Create(Thing thing)
        {
            if (!ValidateCreate(thing))
                return null;

            thing.Id = _repo.Keys.Max() + 1;
            _repo.Add(thing.Id, thing);

            var result = new Result<Thing>
                             {
                                 Entity = thing,
                                 ResultType = ResultType.Success
                             };
            return HandleResult(RestfulAction.Create, result);
        }

        [Help("Updates a thing")]
        public override ActionResult Update(int id, Thing thing)
        {
            if (!ValidateUpdate(id, thing))
                return null;

            Result<Thing> result;
            if (!_repo.ContainsKey(id))
            {
                result = new Result<Thing>
                             {
                                 ErrorMessage = string.Format("Thing {0} not found", thing.Id),
                                 ResultType = ResultType.ClientError
                             };
            }
            else
            {
                thing.Id = id;
                _repo.Remove(thing.Id);
                _repo.Add(thing.Id, thing);
                result = new Result<Thing>
                             {
                                 Entity = thing,
                                 ResultType = ResultType.Success
                             };
            }

            return HandleResult(RestfulAction.Update, result);
        }
    }
}
