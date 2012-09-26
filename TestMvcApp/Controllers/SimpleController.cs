using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReSTCore.Attributes;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.Routing;
using ReSTCore.Util;

namespace TestMvcApp.Controllers
{
    public class SimpleController : BaseController<string, StringDTO>
    {
        [Help("This gets something")]
        public ActionResult GetSomething()
        {
            var dto = new StringDTO {Value = "Just a string"};
            return HandleGetResult(dto, new HandleResultProperties());
        }

        [Help("This does something")]
        [HttpPost]
        public ActionResult DoSomething()
        {
            var dto = new StringDTO {Value = "Just a string"};
            var result = new Result<StringDTO> {Entity = dto, ResultType = ResultType.Success};
            return HandleResult(RestfulAction.Create, result, new HandleResultProperties { IncludeBodyInNonGetRequest = true });
        }

        [Help("This returns a JsonResult only")]
        public JsonResult GetSomethingJson()
        {
            return Json("{ { json: 'json' } }");
        }
    }
}