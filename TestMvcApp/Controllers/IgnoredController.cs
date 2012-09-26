using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReSTCore.Attributes;
using ReSTCore.Controllers;

namespace TestMvcApp.Controllers
{
    [Help(Ignore = true)]
    public class IgnoredController : RestController
    {
        public ActionResult Index()
        {
            return View("");
        }
    }
}
