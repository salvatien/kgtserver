using Dogs.ViewModels.Data.Models;
using kgtwebClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class DogEventsController : Controller
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };
        private static readonly string BlobStorageBaseAddress = ConfigurationManager.AppSettings["BlobStorageBaseAddress"];


        public async Task<ActionResult> Index(int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync($"dogEvents/GetAllByDogId?dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogEvents = JsonConvert.DeserializeObject<List<DogEventModel>>(responseData);

                ViewBag.Id = dogId;
                ViewBag.RawData = responseData;

                return View(dogEvents);
            }
            
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        public async Task<ActionResult> DogEvent (int eventId, int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            var blobTrackLinkBase = $"{BlobStorageBaseAddress}/tracks/";
            HttpResponseMessage responseMessage =
                await client.GetAsync($"dogevents/dogevent?eventId={eventId}&dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogEvent = JsonConvert.DeserializeObject<DogEventModel>(responseData);
                var dogEventViewModel = new DogEventViewModel()
                {
                    Dog = dogEvent.Dog,
                    DogId = dogEvent.DogId,
                    DogTrackFilename = String.IsNullOrWhiteSpace(dogEvent.DogTrackBlobUrl) ? null : dogEvent.DogTrackBlobUrl
                        .Remove(dogEvent.DogTrackBlobUrl.IndexOf(blobTrackLinkBase), blobTrackLinkBase.Length),
                    LostPerson = dogEvent.LostPerson,
                    LostPersonTrackFilename = String.IsNullOrWhiteSpace(dogEvent.LostPersonTrackBlobUrl) ? null : dogEvent.LostPersonTrackBlobUrl
                        .Remove(dogEvent.LostPersonTrackBlobUrl.IndexOf(blobTrackLinkBase), blobTrackLinkBase.Length),
                    Notes = dogEvent.Notes,
                    Event = dogEvent.Event,
                    EventId = dogEvent.EventId,
                    Weather = dogEvent.Weather
                };
                if (!String.IsNullOrEmpty(dogEvent.DogTrackBlobUrl))
                {
                    var webRequestDogTrack = WebRequest.Create(dogEvent.DogTrackBlobUrl);
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

                            dogEventViewModel.DogTrackPoints = t;
                            //TODO: fill other fields DogEventViewModel class (dog part)
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = e.Message;
                        return View("Error");
                    }
                }
                if (!String.IsNullOrEmpty(dogEvent.LostPersonTrackBlobUrl))
                {
                    var webRequestLostPersonTrack = WebRequest.Create(dogEvent.LostPersonTrackBlobUrl);
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

                            dogEventViewModel.LostPersonTrackPoints = t;
                            //TODO: fill other fields DogEventViewModel class (person part)

                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = e.Message;
                        return View("Error");
                    }
                }
                return View(dogEventViewModel);
            }
            else
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UpdateTracks(int dogId, int eventId, string lostPersonTrackFileName, string dogTrackFileName, Trkseg lostPersonTrackPoints, Trkseg dogTrackPoints)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());

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
            var responseMessage = client.PostAsync("DogEvents/Upload", form).Result;

            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                return RedirectToAction("Event", new { dogId, eventId });
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Update(DogEventViewModel model)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            var lostPersonTrackPoints = new Trkseg { Trkpt = model.LostPersonTrackPoints };
            var dogTrackPoints = new Trkseg { Trkpt = model.DogTrackPoints };
            UpdateTracks(model.DogId, model.EventId, model.LostPersonTrackFilename, model.DogTrackFilename, lostPersonTrackPoints, dogTrackPoints);

            var dogEventModel = new DogEventModel
            {
                LostPerson = model.LostPerson,
                Notes = model.Notes,
                Weather = model.Weather,
                DogId = model.DogId,
                EventId = model.EventId
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + $"dogevents/dogevent?dogId={model.DogId}&eventId={model.EventId}");

            var dogEventSerialized = JsonConvert.SerializeObject(dogEventModel);


            message.Content = new StringContent(dogEventSerialized, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                message.Dispose();
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var definition = new { DogId = "", EventId = "" };
                var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                return RedirectToAction("DogEvent", new { dogId = ids.DogId, eventId = ids.EventId });
            }
            else    // msg why not ok
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }


        }

        [HttpPost]
        public ActionResult AddDogEvent(DogEventModel model, HttpPostedFileBase lostPersonTrackFile, HttpPostedFileBase dogTrackFile)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać wydarzenie psa." });

            if (lostPersonTrackFile != null && dogTrackFile != null)
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

                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());

                var response = client.PostAsync("DogEvents/Upload", form).Result;

                if (response.IsSuccessStatusCode)
                {
                    //get blob urls - is it that simple or it has to be returned?

                    var lostPersonTrackBlobUrl = $"{BlobStorageBaseAddress}/tracks/" + lostPersonFileName;
                    var dogTrackBlobUrl = $"{BlobStorageBaseAddress}/tracks/" + dogFileName;

                    //add blob urls to model 
                    model.LostPersonTrackBlobUrl = lostPersonTrackBlobUrl;
                    model.DogTrackBlobUrl = dogTrackBlobUrl;
                }
                else
                {
                    ViewBag.Message = "Upload failed. Reason: " + response.StatusCode;
                    return View("Error");
                }
            }
            //add dogevent
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "dogevents/");

            var dogEventSerialized = JsonConvert.SerializeObject(model);

            message.Content = new StringContent(dogEventSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var definition = new { DogId = "", EventId = "" };
                var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                return RedirectToAction("DogEvent", new { dogId = ids.DogId, eventId = ids.EventId });
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                message.Dispose();
                ViewBag.Message = responseMessage.StatusCode;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult AddDogToEvent(int eventId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać wydarzenie psa." });

            return View(new DogEventModel { EventId = eventId });
        }

        public JsonResult DeleteDogEvent(int? dogId, int? eventId)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return Json(new { success = false, errorCode = "403" });
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress
                                        + $"dogevents/dogevent?eventId={eventId}&dogId={dogId}");
            //message.Content = new StringContent(id.ToString(), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return Json(new { success = true, dogId = dogId.ToString(), eventId = eventId.ToString() });
            }
            else    // wiadomosc czego się nie udałos
            {
                message.Dispose();
                return Json(new { success = false, errorCode = responseMessage.StatusCode });
            }

        }

        [HttpGet]
        public ActionResult AddDogEvent(int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać wydarzenie psa." });
            return View(new DogEventModel { DogId = dogId });
        }


    }
}