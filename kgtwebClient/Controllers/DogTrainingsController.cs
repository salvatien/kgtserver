using Dogs.ViewModels.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace kgtwebClient.Controllers
{
    public class DogTrainingsController : Controller
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
            DogTrainingViewModel trainingTracepoints = new DogTrainingViewModel();

            var dogTrack = "~/Images/Ślad_Pok8-12-08-090130.gpx";
            var personTrack = "~/Images/Ślad_Pok8-12-08-084457.gpx";

            TextReader textReader = new StreamReader(Server.MapPath(dogTrack));

            //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XDocument gpxDoc = XDocument.Load(textReader);
            var serializer = new XmlSerializer(typeof(Gpx));
            var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());
            var t = gpx.Trk.Trkseg.Trkpt;

            trainingTracepoints.DogTrackPoints = t;


            TextReader textReader2 = new StreamReader(Server.MapPath(personTrack));
            XDocument gpxDoc2 = XDocument.Load(textReader2);
            var serializer2 = new XmlSerializer(typeof(Gpx));
            var gpx2 = (Gpx)serializer2.Deserialize(gpxDoc2.Root.CreateReader());
            var t2 = gpx2.Trk.Trkseg.Trkpt;

            trainingTracepoints.LostPersonTrackPoints = t2;

            return View(trainingTracepoints);
        }

        public ActionResult TrainingTest(int dogId, int trainingId)
        {

            //here there will be a call to server web api to get training's viewmodel, including path hardcoded below

            var webRequest = WebRequest.Create(@"https://kgtstorage.blob.core.windows.net/tracks/file06cd3cb0-33fa-477d-861e-86b23d717ad1");

            try
            {
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    XDocument gpxDoc = XDocument.Load(reader);
                    var serializer = new XmlSerializer(typeof(Gpx));
                    var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());
                    var t = gpx.Trk.Trkseg.Trkpt;

                    var model = new DogTrainingViewModel
                    {
                        DogTrackPoints = t,
                        LostPersonTrackPoints = t
                    };
                    return View(model);
                }
            }
            catch(Exception e)
            {
                return View();
            }

        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {

            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            var stream = file.InputStream;
            var streamContent = new StreamContent(stream);
            var imageContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var fileName = file.FileName + Guid.NewGuid().ToString();
            form.Add(imageContent, fileName, Path.GetFileName(fileName));
            var response = httpClient.PostAsync("dogtrainings/upload", form).Result;


            //using (var client = new HttpClient())
            //{
            //    using (var content = new MultipartFormDataContent())
            //    {
            //        byte[] Bytes = new byte[file.InputStream.Length + 1];
            //        file.InputStream.Read(Bytes, 0, Bytes.Length);
            //        var fileContent = new ByteArrayContent(Bytes);
            //        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = file.FileName };
            //        content.Add(fileContent);
            //        var requestUri = "http://localhost:12321/api/trainings/upload";
            //        var result = client.PostAsync(requestUri, content).Result;
            //        if (result.StatusCode == System.Net.HttpStatusCode.Created)
            //        {
            //            Console.WriteLine("Success");

            //        }
            //        else
            //        {
            //            ViewBag.Failed = "Failed !" + result.Content.ToString();
            //        }
            //    }
            //}
            return View();
        }

        [HttpGet]
        public ActionResult UploadTest()
        {
            return View();
        }
    }
}