using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.Routing;
using ReSTCore.Util;

namespace TestMvcApp.Controllers
{
    public class SimpleController : BaseController<string, StringDTO>
    {
        public ActionResult GetSomething()
        {
            var dto = new StringDTO {Value = "Just a string"};
            return HandleGetResult(dto, new HandleResultProperties());
        }

        public ActionResult DoSomething()
        {
            var dto = new StringDTO {Value = "Just a string"};
            var result = new Result<StringDTO> {Entity = dto, ResultType = ResultType.Success};
            return HandleResult(RestfulAction.Create, result, new HandleResultProperties { IncludeBodyInNonGetRequest = true });
        }
    }
}