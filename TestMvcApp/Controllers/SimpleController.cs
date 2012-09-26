using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReSTCore.Attributes;
using ReSTCore.Controllers;

namespace TestMvcApp.Controllers
{
    public class SimpleController : RestController
    {
        [Help("This gets something")]
        public ActionResult GetSomething()
        {
            return View("");
        }

        [Help("This does something")]
        [HttpPost]
        public ActionResult DoSomething()
        {
            return View("");
        }

        [Help("This returns a JsonResult only")]
        public JsonResult GetSomethingJson()
        {
            return Json("{ { json: 'json' } }");
        }
    }
}