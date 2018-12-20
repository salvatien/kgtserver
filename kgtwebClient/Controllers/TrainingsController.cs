using Dogs.ViewModels.Data.Models;
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
            DogTrainingModel trainingTracepoints = new DogTrainingModel();

            DogTraining training = new DogTraining()
            {
                DogTrack = "~/Images/Ślad_Pok8-12-08-090130.gpx",
                PersonTrack = "~/Images/Ślad_Pok8-12-08-084457.gpx"
            };

            TextReader textReader = new StreamReader(Server.MapPath(training.DogTrack));

            //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XDocument gpxDoc = XDocument.Load(textReader);
            var serializer = new XmlSerializer(typeof(Gpx));
            var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());
            var t = gpx.Trk.Trkseg.Trkpt;

            trainingTracepoints.DogTrackpoints = t;


            TextReader textReader2 = new StreamReader(Server.MapPath(training.PersonTrack));
            XDocument gpxDoc2 = XDocument.Load(textReader2);
            var serializer2 = new XmlSerializer(typeof(Gpx));
            var gpx2 = (Gpx)serializer2.Deserialize(gpxDoc2.Root.CreateReader());
            var t2 = gpx2.Trk.Trkseg.Trkpt;

            trainingTracepoints.PersonTrackpoints = t2;

            return View(trainingTracepoints);
        }
    }
}