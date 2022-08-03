using Dogs.ViewModels.Data.Models;
using kgtwebClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };
        private static readonly string BlobStorageBaseAddress = ConfigurationManager.AppSettings["BlobStorageBaseAddress"];


        // GET: Trainings
        // [HttpGet("{dogId}")]
        public async Task<ActionResult> Index(int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by przeglądać tę sekcję" });
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync($"dogtrainings/GetAllByDogId?dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogTrainings = JsonConvert.DeserializeObject<List<DogTrainingModel>>(responseData);
                

                ViewBag.RawData = responseData;
                ViewBag.Id = dogId;
                return View(dogTrainings);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        

        public async Task<ActionResult> Training(int dogId, int trainingId)
       {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by przeglądać tę sekcję" });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            var blobTrackLinkBase = $"{BlobStorageBaseAddress}/tracks/";
            HttpResponseMessage responseMessage =
                await client.GetAsync($"dogtrainings/training?trainingId={trainingId}&dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogTraining = JsonConvert.DeserializeObject<DogTrainingModel>(responseData);
                var dogTrainingViewModel = new DogTrainingViewModel() {
                    Comments = dogTraining.Comments,
                    Dog = dogTraining.Dog,
                    DogId = dogTraining.DogId,
                    DogTrackFilename = dogTraining.DogTrackBlobUrl
                        .Remove(dogTraining.DogTrackBlobUrl.IndexOf(blobTrackLinkBase), blobTrackLinkBase.Length),
                    LostPerson = dogTraining.LostPerson,
                    LostPersonTrackFilename = dogTraining.LostPersonTrackBlobUrl
                        .Remove(dogTraining.LostPersonTrackBlobUrl.IndexOf(blobTrackLinkBase), blobTrackLinkBase.Length),
                    Notes = dogTraining.Notes,
                    Training = dogTraining.Training,
                    TrainingId = dogTraining.TrainingId,
                    Weather = dogTraining.Weather,
                    GroundType = dogTraining.GroundType,
                    AdditionalPictureBlobUrl = dogTraining.AdditionalPictureBlobUrl
                };
                if (!String.IsNullOrEmpty(dogTraining.DogTrackBlobUrl))
                {
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
                            if (trkseg != null && trkseg.Trkpt.Any())
                            {
                                var t = trkseg.Trkpt;
                                dogTrainingViewModel.DogTrackPoints = t;
                                //dogTrainingViewModel.DogTrackLength = DogTrainingHelper.CalculateGPSTrackLength(trkseg);
                                dogTrainingViewModel.Duration = DogTrainingHelper.CalculateDuration(trkseg);
                                dogTrainingViewModel.TimeOfDogStart = DogTrainingHelper.CalculateGPSTrackStartTime(trkseg);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = e.Message;
                        return View("Error");
                    }
                }
                if (!String.IsNullOrEmpty(dogTraining.LostPersonTrackBlobUrl))
                {
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

                            if (trkseg != null && trkseg.Trkpt.Any())
                            {
                                var t = trkseg.Trkpt;
                                dogTrainingViewModel.LostPersonTrackPoints = t;
                                //dogTrainingViewModel.LostPersonTrackLength = DogTrainingHelper.CalculateGPSTrackLength(trkseg);
                                dogTrainingViewModel.TimeOfLostPersonStart = DogTrainingHelper.CalculateGPSTrackStartTime(trkseg);
                                dogTrainingViewModel.DelayTime = dogTrainingViewModel.TimeOfDogStart - dogTrainingViewModel.TimeOfLostPersonStart;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = e.Message;
                        return View("Error");
                    }
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
        public ActionResult UpdateTracks(int dogId, int trainingId, string lostPersonTrackFileName, string dogTrackFileName, Trkseg lostPersonTrackPoints, Trkseg dogTrackPoints)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account");
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

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

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());

            var responseMessage = client.PostAsync("DogTrainings/Upload", form).Result;

            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                return RedirectToAction("Training", new { dogId, trainingId });
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                ViewBag.Message = "code: " + responseMessage.StatusCode;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Update(DogTrainingViewModel model, HttpPostedFileBase imageFile)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account");
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });


            if (imageFile != null)
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                var imageStreamContent = new StreamContent(imageFile.InputStream);
                var byteArrayImageContent = new ByteArrayContent(imageStreamContent.ReadAsByteArrayAsync().Result);
                byteArrayImageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                var imageFileName = imageFile.FileName + Guid.NewGuid().ToString();
                form.Add(byteArrayImageContent, imageFileName, Path.GetFileName(imageFileName));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
                var response = client.PostAsync("DogTrainings/UploadImage", form).Result;

                if (response.IsSuccessStatusCode)
                {
                    model.AdditionalPictureBlobUrl = $"{BlobStorageBaseAddress}/images/" + imageFileName;
                }
                else
                {
                    ViewBag.Message = response.StatusCode;
                    return View("Error");
                }
            }

            var lostPersonTrackPoints = new Trkseg { Trkpt = model.LostPersonTrackPoints };
            var dogTrackPoints = new Trkseg { Trkpt = model.DogTrackPoints };
            UpdateTracks(model.DogId, model.TrainingId, model.LostPersonTrackFilename, model.DogTrackFilename, lostPersonTrackPoints, dogTrackPoints);

            var dogTrainingModel = new DogTrainingModel
            {
                LostPerson = model.LostPerson,
                Notes = model.Notes,
                Weather = model.Weather,
                GroundType = model.GroundType,
                Comments = model.Comments,
                DogId = model.DogId,
                TrainingId = model.TrainingId,
                LostPersonTrackLength = model.LostPersonTrackLength,
                DelayTime = model.DelayTime,
                AdditionalPictureBlobUrl = model.AdditionalPictureBlobUrl
            };
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + $"dogtrainings/training?dogId={model.DogId}&trainingId={model.TrainingId}");
            
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
        public ActionResult Add(DogTrainingModel model, HttpPostedFileBase lostPersonTrackFile, HttpPostedFileBase dogTrackFile, HttpPostedFileBase imageFile)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account");
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            MultipartFormDataContent additionalPhotoForm = new MultipartFormDataContent();
            if (imageFile != null)
            {
                var additionalPictureStream = imageFile.InputStream;
                var additionalPictureStreamContent = new StreamContent(additionalPictureStream);
                var additionalPictureByteArrayContent =
                    new ByteArrayContent(additionalPictureStreamContent.ReadAsByteArrayAsync().Result);
                additionalPictureByteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                var additionalPictureFileName = imageFile.FileName + Guid.NewGuid().ToString();
                additionalPhotoForm.Add(additionalPictureByteArrayContent, additionalPictureFileName, Path.GetFileName(additionalPictureFileName));

                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());

                var additionalPhotoUploadResponse = client.PostAsync("DogTrainings/UploadImage", additionalPhotoForm).Result;

                if (!additionalPhotoUploadResponse.IsSuccessStatusCode)
                {
                    ViewBag.Message = "image upload failed. Reason: " + additionalPhotoUploadResponse.StatusCode;
                    return View("Error");
                }

                else
                {
                    model.AdditionalPictureBlobUrl = $"{BlobStorageBaseAddress}/images/" + additionalPictureFileName;
                }
            }

            var originalLostPersonTrackStream = lostPersonTrackFile.InputStream;
            var cleanedLostPersonTrackStream = new MemoryStream();
            DateTime lostPersonStartTime = new DateTime();
            double lostPersonTrackLength = 0.0;
            DateTime dogStartTime = new DateTime();
            using (var reader = new StreamReader(originalLostPersonTrackStream))
            {
                XDocument gpxDoc = XDocument.Load(reader);
                var originalFileSerializer = new XmlSerializer(typeof(Gpx));
                var gpx = (Gpx)originalFileSerializer.Deserialize(gpxDoc.Root.CreateReader());
                var trkSeg = gpx.Trk.Trkseg;

                //calculate lost person track length and time of lost person start
                lostPersonStartTime = DogTrainingHelper.CalculateGPSTrackStartTime(trkSeg);
                //lostPersonTrackLength = DogTrainingHelper.CalculateGPSTrackLength(trkSeg);

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

                //calculate time of dog start
                dogStartTime = DogTrainingHelper.CalculateGPSTrackStartTime(trkSeg);

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

            var response = client.PostAsync("DogTrainings/Upload", form).Result;

            if (response.IsSuccessStatusCode)
            {
                //get blob urls - is it that simple or it has to be returned?

                var lostPersonTrackBlobUrl = $"{BlobStorageBaseAddress}/tracks/" + lostPersonFileName;
                var dogTrackBlobUrl = $"{BlobStorageBaseAddress}/tracks/" + dogFileName;

                //add blob urls to model 
                model.LostPersonTrackBlobUrl = lostPersonTrackBlobUrl;
                model.DogTrackBlobUrl = dogTrackBlobUrl;

                //add lost person track length and delay time to model
                model.DelayTime = dogStartTime - lostPersonStartTime;
                //model.LostPersonTrackLength = lostPersonTrackLength;

                //add dogtraining
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
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
                    ViewBag.Message = responseMessage.StatusCode;
                    return View("Error");
                }

            }

            ViewBag.Message = "upload failed. Reason: " + response.StatusCode;
            return View("Error");
        }

        [HttpGet]
        public ActionResult Add(int trainingId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by przeglądać tę sekcję" });
            return View(new DogTrainingModel { TrainingId = trainingId});
        }

        [HttpGet]
        public ActionResult AddTrainingToDog(int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by przeglądać tę sekcję" });
            return View(new DogTrainingModel { DogId = dogId });
        }

        public JsonResult DeleteDogTraining(int? dogId, int? trainingId)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = 403});
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return Json(new { success = false, errorCode = 403 });
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
                                        + $"dogtrainings/training?trainingId={trainingId}&dogId={dogId}");
            //message.Content = new StringContent(id.ToString(), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return Json(new { success = true,  dogId = dogId.ToString(), trainingId = trainingId.ToString() });
            }
            else    // wiadomosc czego się nie udałos
            {
                message.Dispose();
                return Json(false);
            }

        }

        ////[HttpPost]
        //public JsonResult CalculateGPSTrackLength(List<TrkptModel> trkpts)
        //{
        //    if (!LoginHelper.IsAuthenticated())
        //        return Json(new { success = false, errorCode = 403 });
        //    else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
        //        return Json(new { success = false, errorCode = 403 });

        //    var trkptList = DogTrainingHelper.ConvertTrkptModelListToTrkptList(trkpts);
        //    var length = DogTrainingHelper.CalculateGPSTrackLength(new Trkseg() { Trkpt = trkptList });
        //    return Json(new { success = true, result = length });
        //}
        ///?
        //public JsonResult CalculateDelayTime(List<TrkptModel> personTrkpts, List<TrkptModel> dogTrkpts)
        //{
        //    if (!LoginHelper.IsAuthenticated())
        //        return Json(new { success = false, errorCode = 403 });
        //    else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
        //        return Json(new { success = false, errorCode = 403 });

        //    var dogTrkptList = DogTrainingHelper.ConvertTrkptModelListToTrkptList(dogTrkpts);
        //    var personTrkptList = DogTrainingHelper.ConvertTrkptModelListToTrkptList(personTrkpts);

        //    var delay = DogTrainingHelper.CalculateDelayTime(new Trkseg() { Trkpt = dogTrkptList }, new Trkseg() { Trkpt = personTrkptList});
        //    return Json(new { success = true, result = delay });
        //}

        public JsonResult CalculateDuration(List<TrkptModel> trkpts)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = 403 });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return Json(new { success = false, errorCode = 403 });

            var trkptList = DogTrainingHelper.ConvertTrkptModelListToTrkptList(trkpts);
            var duration = DogTrainingHelper.CalculateDuration(new Trkseg() { Trkpt = trkptList });
            return Json(new { success = true, result = duration.ToString() });
        }

        public JsonResult CalculateGPSTrackStartTime(List<TrkptModel> trkpts)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = 403 });
            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return Json(new { success = false, errorCode = 403 });

            var trkptList = DogTrainingHelper.ConvertTrkptModelListToTrkptList(trkpts);
            var startTime = DogTrainingHelper.CalculateGPSTrackStartTime(new Trkseg() { Trkpt = trkptList });
            return Json(new { success = true, result = startTime.ToString() });
        }
    }
}