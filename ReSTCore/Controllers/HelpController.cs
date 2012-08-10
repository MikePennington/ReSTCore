using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ReSTCore.Models;

namespace ReSTCore.Controllers
{
    public class HelpController : Controller
    {
        public ActionResult Index()
        {
            var verifyModel = new HelpModel();
            return View("~/Views/Rest/Help.cshtml", verifyModel);
        }
    }
}
