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
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {

            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            var stream = file.InputStream;
            var streamContent = new StreamContent(stream);

            var imageContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            form.Add(imageContent, "image", Path.GetFileName("your file name"));
            var response = httpClient.PostAsync("http://localhost:12321/api/trainings/upload", form).Result;


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