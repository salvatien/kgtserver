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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Ziemniak()
        {
            ViewBag.Message = "zIeminak";
            return View();
        }

        public ActionResult Error(string error)
        {
            ViewBag.Message = error;
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}