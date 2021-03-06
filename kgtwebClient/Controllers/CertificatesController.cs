﻿using Dogs.ViewModels.Data.Models;
using kgtwebClient.Helpers;
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

namespace kgtwebClient.Controllers
{
    public class CertificatesController : Controller
    {

        //The URL of the WEB API Service
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        // get all dogs from db
        public async Task<ActionResult> Index()
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("certificates/");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var certificates = JsonConvert.DeserializeObject<List<CertificateModel>>(responseData);
                    
                ViewBag.RawData = responseData;

                return View(certificates);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        public async Task<ActionResult> Certificate(int id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("certificates/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var certificate = JsonConvert.DeserializeObject<CertificateModel>(responseData);

                return View(certificate);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }
        [HttpGet]
        public ActionResult AddCertificate()
        {
            // var guides = GuideHelpers.GetAllGuidesIdAndName();
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if(!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> /*Task<ActionResult>*/ AddCertificate(CertificateModel addedCertificate)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            /* for put $ post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "certificates/");

            var certificateSerialized = JsonConvert.SerializeObject(addedCertificate);

            message.Content = new StringContent(certificateSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return RedirectToAction("Certificate", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
            }
            else    // msg why not ok
            {
                message.Dispose();
                ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
                return View("Error");
            }

        }

        public JsonResult DeleteCertificate(int? id)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });
            else if (!LoginHelper.IsCurrentUserAdmin())
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
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "certificates/" + id.ToString());
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
                return Json(new { success = false, errorCode = responseMessage.StatusCode });
            }

        }

        //TODO metody UpdateDog(ta niżej) i Dog robią to samo -> wyrzucić środek do innej metody i wywoływać ją sobie wewnątrz
        [HttpGet]
        public async Task<ActionResult> UpdateCertificate(int id)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("certificates/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var certificate = JsonConvert.DeserializeObject<CertificateModel>(responseData);


                return View(certificate);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCertificate(CertificateModel updatedCertificate)    //? -> może być null
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            else if (!LoginHelper.IsCurrentUserAdmin())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by dodać certyfikat." });


            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "certificates/" + updatedCertificate.CertificateId.ToString());
            var certificateSerialized = JsonConvert.SerializeObject(updatedCertificate);

            message.Content = new StringContent(certificateSerialized, System.Text.Encoding.UTF8, "application/json"); //dog serialized id.ToString()
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację


                message.Dispose();
                return RedirectToAction("Certificate", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });

            }
            else    // wiadomosc czego się nie udało
            {

                message.Dispose();
                ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
                return View("Error");
            }

        }


    }
}