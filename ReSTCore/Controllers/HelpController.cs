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
            var model = new IndexModel();
            return View("~/Views/RestCore/Index.cshtml", model);
        }

        public ActionResult DTO()
        {
            var model = new DtoModel();
            return View("~/Views/RestCore/Dto.cshtml", model);
        }
    }
}
