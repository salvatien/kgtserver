using Dogs.ViewModels.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using kgtwebClient.Helpers;
using System.IO;

namespace kgtwebClient.Controllers
{
    public class DogsController : Controller
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        // get all dogs from db
        public async Task<ActionResult> Index()
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //not necessary - its not blocked on server, but better to add it just in case we want to block it
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("dogs/");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogs = JsonConvert.DeserializeObject<List<DogModel>>(responseData);

                var dogsList = new DogsListModel
                {
                    ListOfDogs = dogs
                };

                ViewBag.RawData = responseData;

                return View(dogsList);
            }
            return View();
        }
        public async Task<ActionResult> Dog(int id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("dogs/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dog = JsonConvert.DeserializeObject<DogModel>(responseData);

                return View(dog);
            }
            return View();
        }
        [HttpGet]
        public ActionResult AddDog()
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            // var guides = GuideHelpers.GetAllGuidesIdAndName();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> /*Task<ActionResult>*/ AddDog(DogModel addedDog, HttpPostedFileBase imageFile)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            MultipartFormDataContent form = new MultipartFormDataContent();
            var imageStreamContent = new StreamContent(imageFile.InputStream);
            var byteArrayImageContent = new ByteArrayContent(imageStreamContent.ReadAsByteArrayAsync().Result);
            byteArrayImageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            var imageFileName = imageFile.FileName + Guid.NewGuid().ToString();
            form.Add(byteArrayImageContent, imageFileName, Path.GetFileName(imageFileName));

            var response = client.PostAsync("Dogs/Upload", form).Result;

            if (response.IsSuccessStatusCode)
            {
                //get blob urls - is it that simple or it has to be returned?

                var imageBlobUrl = @"https://kgtstorage.blob.core.windows.net/images/" + imageFileName;

                //add blob urls to model 
                addedDog.PhotoBlobUrl = imageBlobUrl;

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "dogs/");

                var dogSerialized = JsonConvert.SerializeObject(addedDog);

                message.Content = new StringContent(dogSerialized, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
                if (responseMessage.IsSuccessStatusCode)    //200 OK
                {
                    //display info
                    message.Dispose();
                    return RedirectToAction("Dog", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
                    //return View("Dog", responseMessage.Content);
                }
                else    // msg why not ok
                {
                    message.Dispose();
                    ViewBag.Message = response.StatusCode;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Message = response.StatusCode;
                return View("Error");
            }
        }

        public JsonResult DeleteDog(int? id)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = 401 });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "dogs/" + id.ToString());
            message.Content = new StringContent(id.ToString(), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return Json(new { success = true, id = id.ToString() });
            }
            else    // wiadomosc czego się nie udałos
            {
                message.Dispose();
                return Json(false);
            }

        }

        //TODO metody UpdateDog(ta niżej) i Dog robią to samo -> wyrzucić środek do innej metody i wywoływać ją sobie wewnątrz
        [HttpGet]
        public async Task<ActionResult> UpdateDog(int id)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("dogs/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var dog = JsonConvert.DeserializeObject<DogModel>(responseData);


                return View(dog);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateDog(DogModel updatedDog, HttpPostedFileBase imageFile)    //? -> może być null
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
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
                var response = client.PostAsync("Dogs/Upload", form).Result;

                if (response.IsSuccessStatusCode)
                {
                    //get blob urls - is it that simple or it has to be returned?

                    var imageBlobUrl = @"https://kgtstorage.blob.core.windows.net/images/" + imageFileName;

                    //add blob urls to model 
                    updatedDog.PhotoBlobUrl = imageBlobUrl;

                }
                else
                {
                    ViewBag.Message = response.StatusCode;
                    return View("Error");
                }
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "dogs/" + updatedDog.DogId.ToString());

            var dogSerialized = JsonConvert.SerializeObject(updatedDog);


            message.Content = new StringContent(dogSerialized, System.Text.Encoding.UTF8, "application/json"); //dog serialized id.ToString()
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację


                message.Dispose();
                return RedirectToAction("Dog", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });

            }
            else    // wiadomosc czego się nie udało
            {
                message.Dispose();
                ViewBag.Message = responseMessage.StatusCode;
                return View("Error");
            }
        }

    }
}
