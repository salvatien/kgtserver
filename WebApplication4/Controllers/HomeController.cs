using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {

        const string SessionName = "_Name";
        const string SessionAge = "_Age";
        [HttpGet]
        public string Index()
        {
            HttpContext.Session.SetString(SessionName, "Januszek");
            HttpContext.Session.SetInt32(SessionAge, 24);
            return HttpContext.Session.GetString(SessionName);
        }
        [HttpGet("getsessionname")]
        public string GetSessionName()
        {
            return HttpContext.Session.GetString(SessionName);
        }

    }
}