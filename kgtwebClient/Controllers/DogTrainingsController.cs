using Dogs.ViewModels.Data.Models;
using Newtonsoft.Json;
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

        public async Task<ActionResult> TrainingTest(int dogId, int trainingId)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = 
                await client.GetAsync($"dogtrainings/training?trainingId={trainingId}&dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogTraining = JsonConvert.DeserializeObject<DogTrainingModel>(responseData);
                var dogTrainingViewModel = new DogTrainingViewModel();

                var webRequestDogTrack = WebRequest.Create(dogTraining.DogTrackBlobUrl);
                try
                {
                    using (var response = webRequestDogTrack.GetResponse())
                    using (var content = response.GetResponseStream())
                    using (var reader = new StreamReader(content))
                    {
                        XDocument gpxDoc = XDocument.Load(reader);
                        var serializer = new XmlSerializer(typeof(Gpx));
                        var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());
                        var t = gpx.Trk.Trkseg.Trkpt;

                        dogTrainingViewModel.DogTrackPoints = t;
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Message = e.Message;
                    return View("Error");
                }

                var webRequestLostPersonTrack = WebRequest.Create(dogTraining.LostPersonTrackBlobUrl);
                try
                {
                    using (var response = webRequestLostPersonTrack.GetResponse())
                    using (var content = response.GetResponseStream())
                    using (var reader = new StreamReader(content))
                    {
                        XDocument gpxDoc = XDocument.Load(reader);
                        var serializer = new XmlSerializer(typeof(Gpx));
                        var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());
                        var t = gpx.Trk.Trkseg.Trkpt;

                        dogTrainingViewModel.LostPersonTrackPoints = t;
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Message = e.Message;
                    return View("Error");
                }

                return View(dogTrainingViewModel);
            }
            else
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UploadFile(DogTrainingModel model, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {

            MultipartFormDataContent form = new MultipartFormDataContent();
            var stream1 = file1.InputStream;
            var streamContent1 = new StreamContent(stream1);
            var imageContent1 = new ByteArrayContent(streamContent1.ReadAsByteArrayAsync().Result);
            imageContent1.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var fileName1 = file1.FileName + Guid.NewGuid().ToString();
            form.Add(imageContent1, fileName1, Path.GetFileName(fileName1));
            var stream2 = file2.InputStream;
            var streamContent2 = new StreamContent(stream2);
            var imageContent2 = new ByteArrayContent(streamContent2.ReadAsByteArrayAsync().Result);
            imageContent2.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var fileName2 = file2.FileName + Guid.NewGuid().ToString();
            form.Add(imageContent2, fileName2, Path.GetFileName(fileName2));
            var response = client.PostAsync("DogTrainings/Upload", form).Result;

            if (response.IsSuccessStatusCode)
            {
                //get blob urls - is it that simple or it has to be returned?

                var track1 = @"https://kgtstorage.blob.core.windows.net/tracks/" + fileName1;
                var track2 = @"https://kgtstorage.blob.core.windows.net/tracks/" + fileName2;

                //add blob urls to model 
                model.LostPersonTrackBlobUrl = track1;
                model.DogTrackBlobUrl = track2;
                //add dogtraining
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "dogtrainings/");

                var dogTrainingSerialized = JsonConvert.SerializeObject(model);

                message.Content = new StringContent(dogTrainingSerialized, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
                if (responseMessage.IsSuccessStatusCode)    //200 OK
                {
                    //display info
                    message.Dispose();
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    var definition = new { DogId = "", TrainingId = "" };
                    var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                    return RedirectToAction("TrainingTest", new { dogId = ids.DogId, trainingId = ids.TrainingId });
                    //return View("Dog", responseMessage.Content);
                }
                else    // msg why not ok
                {
                    message.Dispose();
                    return View(/*error*/);
                }

            }



            return View();
        }

        [HttpGet]
        public ActionResult UploadTest(int trainingId)
        {
            return View(new DogTrainingModel { TrainingId = trainingId});
        }
    }
}