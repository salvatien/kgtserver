﻿using Dogs.ViewModels.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace kgtwebClient.Controllers
{
    public class TrainingsController : Controller
    {


        //The URL of the WEB API Service
        #if DEBUG
        static string url = "http://localhost:12321/api/";
        #else
        static string url = "http://kgt.azurewebsites.net/api/";
        #endif
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        // GET: Trainings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Training()
        {
            TextReader textReader = new StreamReader(Server.MapPath("~/Images/Cobby.gpx"));

            //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XDocument gpxDoc = XDocument.Load(textReader);
            var serializer = new XmlSerializer(typeof(Gpx));
            var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());


            var t = gpx.Trk[0].Trkseg.Trkpt;
            //var list = new List<Trkpt>();
            //foreach (var pt in t)
            //{
            //    list.Add( )
            //}
            

            return View(t);
        }
    }
}