using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kgtwebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error(string error)
        {
            ViewBag.Message = error;
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}