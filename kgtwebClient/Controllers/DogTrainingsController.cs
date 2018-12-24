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

        //public ActionResult Training()
        //{
        //    DogTrainingViewModel trainingTracepoints = new DogTrainingViewModel();

        //    var dogTrack = "~/Images/Ślad_Pok8-12-08-090130.gpx";
        //    var personTrack = "~/Images/Ślad_Pok8-12-08-084457.gpx";

        //    TextReader textReader = new StreamReader(Server.MapPath(dogTrack));

        //    //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
        //    XDocument gpxDoc = XDocument.Load(textReader);
        //    var serializer = new XmlSerializer(typeof(Gpx));
        //    var gpx = (Gpx)serializer.Deserialize(gpxDoc.Root.CreateReader());
        //    var t = gpx.Trk.Trkseg.Trkpt;

        //    trainingTracepoints.DogTrackPoints = t;


        //    TextReader textReader2 = new StreamReader(Server.MapPath(personTrack));
        //    XDocument gpxDoc2 = XDocument.Load(textReader2);
        //    var serializer2 = new XmlSerializer(typeof(Gpx));
        //    var gpx2 = (Gpx)serializer2.Deserialize(gpxDoc2.Root.CreateReader());
        //    var t2 = gpx2.Trk.Trkseg.Trkpt;

        //    trainingTracepoints.LostPersonTrackPoints = t2;

        //    return View(trainingTracepoints);
        //}

        public async Task<ActionResult> Training(int dogId, int trainingId)
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
                        var serializer = new XmlSerializer(typeof(Trkseg));
                        var trkseg = (Trkseg)serializer.Deserialize(gpxDoc.Root.CreateReader());
                        var t = trkseg.Trkpt;

                        dogTrainingViewModel.DogTrackPoints = t;
                        //TODO: fill other fields DogTrainingViewModel class (dog part)
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
                        var serializer = new XmlSerializer(typeof(Trkseg));
                        var trkseg = (Trkseg)serializer.Deserialize(gpxDoc.Root.CreateReader());
                        var t = trkseg.Trkpt;

                        dogTrainingViewModel.LostPersonTrackPoints = t;
                        //TODO: fill other fields DogTrainingViewModel class (person part)

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
        public ActionResult UpdateTracks(string lostPersonTrackFileName, string dogTrackFileName, Trkseg lostPersonTrackPoints, Trkseg dogTrackPoints)
        {
            var updatedLostPersonTrackStream = new MemoryStream();
            var lostPersonFileSerializer = new XmlSerializer(typeof(Trkseg));
            lostPersonFileSerializer.Serialize(updatedLostPersonTrackStream, lostPersonTrackPoints);
            updatedLostPersonTrackStream.Position = 0;

            var updatedDogTrackStream = new MemoryStream();
            var dogFileSerializer = new XmlSerializer(typeof(Trkseg));
            dogFileSerializer.Serialize(updatedDogTrackStream, dogTrackPoints);
            updatedDogTrackStream.Position = 0;



            MultipartFormDataContent form = new MultipartFormDataContent();
            var lostPersonStreamContent = new StreamContent(updatedLostPersonTrackStream);
            var lostPersonImageContent = new ByteArrayContent(lostPersonStreamContent.ReadAsByteArrayAsync().Result);
            lostPersonImageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(lostPersonImageContent, lostPersonTrackFileName, Path.GetFileName(lostPersonTrackFileName));

            var dogStreamContent = new StreamContent(updatedDogTrackStream);
            var dogImageContent = new ByteArrayContent(dogStreamContent.ReadAsByteArrayAsync().Result);
            dogImageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(dogImageContent, dogTrackFileName, Path.GetFileName(dogTrackFileName));
            var responseMessage = client.PostAsync("DogTrainings/Upload", form).Result;

            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var definition = new { DogId = "", TrainingId = "" };
                var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                return RedirectToAction("Training", new { dogId = ids.DogId, trainingId = ids.TrainingId });
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Update(DogTrainingViewModel model)
        {
            var lostPersonTrackPoints = new Trkseg { Trkpt = model.LostPersonTrackPoints };
            var dogTrackPoints = new Trkseg { Trkpt = model.DogTrackPoints };
            UpdateTracks(model.LostPersonTrackFilename, model.DogTrackFilename, lostPersonTrackPoints, dogTrackPoints);

            var dogTrainingModel = new DogTrainingModel
            {
                LostPerson = model.LostPerson,
                Notes = model.Notes,
                Weather = model.Weather
            };
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + $"dogtrainings?dogId={model.DogId}&trainingId={model.TrainingId}");
            
            var dogTrainingSerialized = JsonConvert.SerializeObject(dogTrainingModel);


            message.Content = new StringContent(dogTrainingSerialized, System.Text.Encoding.UTF8, "application/json"); 
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                message.Dispose();
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var definition = new { DogId = "", TrainingId = "" };
                var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                return RedirectToAction("Training", new { dogId = ids.DogId, trainingId = ids.TrainingId });
            }
            else    // msg why not ok
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }


        }

        [HttpPost]
        public ActionResult Add(DogTrainingModel model, HttpPostedFileBase lostPersonTrackFile, HttpPostedFileBase dogTrackFile)
        {
            var originalLostPersonTrackStream = lostPersonTrackFile.InputStream;
            var cleanedLostPersonTrackStream = new MemoryStream();
            using (var reader = new StreamReader(originalLostPersonTrackStream))
            {
                XDocument gpxDoc = XDocument.Load(reader);
                var originalFileSerializer = new XmlSerializer(typeof(Gpx));
                var gpx = (Gpx)originalFileSerializer.Deserialize(gpxDoc.Root.CreateReader());
                var trkSeg = gpx.Trk.Trkseg;
                var cleanedFileSerializer = new XmlSerializer(typeof(Trkseg));
                cleanedFileSerializer.Serialize(cleanedLostPersonTrackStream, trkSeg);
                cleanedLostPersonTrackStream.Position = 0;
                
            }

            var originalDogTrackStream = dogTrackFile.InputStream;
            var cleanedDogTrackStream = new MemoryStream();
            using (var reader = new StreamReader(originalDogTrackStream))
            {
                XDocument gpxDoc = XDocument.Load(reader);
                var originalFileSerializer = new XmlSerializer(typeof(Gpx));
                var gpx = (Gpx)originalFileSerializer.Deserialize(gpxDoc.Root.CreateReader());
                var trkSeg = gpx.Trk.Trkseg;
                var cleanedFileSerializer = new XmlSerializer(typeof(Trkseg));
                cleanedFileSerializer.Serialize(cleanedDogTrackStream, trkSeg);
                cleanedDogTrackStream.Position = 0;

            }


            MultipartFormDataContent form = new MultipartFormDataContent();
            var lostPersonStreamContent = new StreamContent(cleanedLostPersonTrackStream);
            var lostPersonImageContent = new ByteArrayContent(lostPersonStreamContent.ReadAsByteArrayAsync().Result);
            lostPersonImageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var lostPersonFileName = lostPersonTrackFile.FileName + Guid.NewGuid().ToString();
            form.Add(lostPersonImageContent, lostPersonFileName, Path.GetFileName(lostPersonFileName));

            var dogStreamContent = new StreamContent(cleanedDogTrackStream);
            var dogImageContent = new ByteArrayContent(dogStreamContent.ReadAsByteArrayAsync().Result);
            dogImageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var dogFileName = dogTrackFile.FileName + Guid.NewGuid().ToString();
            form.Add(dogImageContent, dogFileName, Path.GetFileName(dogFileName));
            var response = client.PostAsync("DogTrainings/Upload", form).Result;

            if (response.IsSuccessStatusCode)
            {
                //get blob urls - is it that simple or it has to be returned?

                var lostPersonTrackBlobUrl = @"https://kgtstorage.blob.core.windows.net/tracks/" + lostPersonFileName;
                var dogTrackBlobUrl = @"https://kgtstorage.blob.core.windows.net/tracks/" + dogFileName;

                //add blob urls to model 
                model.LostPersonTrackBlobUrl = lostPersonTrackBlobUrl;
                model.DogTrackBlobUrl = dogTrackBlobUrl;
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
                    return RedirectToAction("Training", new { dogId = ids.DogId, trainingId = ids.TrainingId });
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
        public ActionResult Add(int trainingId)
        {
            return View(new DogTrainingModel { TrainingId = trainingId});
        }
    }
}