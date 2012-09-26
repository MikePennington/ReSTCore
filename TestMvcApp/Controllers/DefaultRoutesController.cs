using System;
using System.Web.Mvc;
using ReSTCore.Attributes;
using ReSTCore.Controllers;
using ReSTCore.DTO;

namespace TestMvcApp.Controllers
{
    public class DefaultRoutesController : TypedRestController<string, RestDTO<string>>
    {
        [Help("Convert method")]
        public ActionResult Convert()
        {
            return View("");
        }

        [Help("Get method")]
        [HttpGet]
        public ActionResult Get()
        {
            return View("");
        }

        [Help("Post method")]
        [HttpPost]
        public ActionResult Post()
        {
            return View("");
        }
    }
}
