using Dogs.ViewModels.Data.Models;
using kgtwebClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace kgtwebClient.Controllers
{
    public class DogCertificatesController : Controller
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        public async Task<ActionResult> Index(int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync($"dogCertificates/GetAllByDogId?dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogCertificates = JsonConvert.DeserializeObject<List<DogCertificateModel>>(responseData);


                ViewBag.RawData = responseData;

                return View(dogCertificates);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        public async Task<ActionResult> DogCertificate(int certificateId, int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync($"dogCertificates/DogCertificate?dogId={dogId}&certificateId={certificateId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogCertificate = JsonConvert.DeserializeObject<DogCertificateModel>(responseData);


                ViewBag.RawData = responseData;

                return View(dogCertificate);
            }
            
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        [HttpPost]
        public ActionResult AddDogCertificate(DogCertificateModel addedDogCertificate)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });

            //add dogtraining
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "dogcertificates/");

            var dogCertificateSerialized = JsonConvert.SerializeObject(addedDogCertificate);

            message.Content = new StringContent(dogCertificateSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var definition = new { DogId = "", CertificateId = "" };
                var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                return RedirectToAction("Index", new { dogId = ids.DogId});
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
        public ActionResult AddDogCertificate(int dogId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });
            return View(new DogCertificateModel { DogId = dogId });
        }

        [HttpGet]
        public ActionResult AddDogToCertificate(int certificateId)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });
            return View(new DogCertificateModel { CertificateId = certificateId });
        }

        public JsonResult DeleteDogCertificate(int? dogId, int? certificateId)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress
                                        + $"dogcertificates/dogcertificate?certificateId={certificateId}&dogId={dogId}");
            //message.Content = new StringContent(id.ToString(), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return Json(new { success = true, dogId = dogId.ToString(), certificateId = certificateId.ToString() });
            }
            else    // wiadomosc czego się nie udałos
            {
                message.Dispose();
                return Json( new { success = false, errorCode = responseMessage.StatusCode });

            }

        }
    }
}