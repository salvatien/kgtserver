using Dogs.ViewModels.Data.Models;
using kgtwebClient.Helpers;
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
    public class TrainingsController : Controller
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        // get all trainings from db
        public async Task<ActionResult> Index()
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            HttpResponseMessage responseMessage = await client.GetAsync("Trainings/");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var trainings = JsonConvert.DeserializeObject<List<TrainingModel>>(responseData);

                if (trainings.Any())
                    return View(trainings);
                else
                    return View();
            }
            ViewBag.Message = "code:" + responseMessage.StatusCode;
            return View("Error");
        }

        public async Task<ActionResult> Training(int id)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("trainings/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var training = JsonConvert.DeserializeObject<TrainingModel>(responseData);

                return View(training);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> AddTraining()
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddTraining(TrainingModel addedTraining)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "trainings/");

            addedTraining.Date = addedTraining.Date.ToUniversalTime();
            var trainingSerialized = JsonConvert.SerializeObject(addedTraining);

            message.Content = new StringContent(trainingSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await client.SendAsync(message);
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return RedirectToAction("Training", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
            }
            else    // msg why not ok
            {
                message.Dispose();
                ViewBag.Message = "code:" + responseMessage.StatusCode;
                return View("Error");
            }

        }

        public JsonResult DeleteTraining(int? id)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = 403 });

            else if (!LoginHelper.IsCurrentUserAdmin())
                return Json(new { success = false, errorCode = 403 });

            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "trainings/" + id.ToString());
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

        [HttpGet]
        public async Task<ActionResult> UpdateTraining(int id)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });

            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("trainings/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var training = JsonConvert.DeserializeObject<TrainingModel>(responseData);


                return View(training);
            }
            ViewBag.Message = "code:" + responseMessage.StatusCode;
            return View("Error");
        }

        [HttpPost]
        public ActionResult UpdateTraining(TrainingModel updatedTraining)    //? -> może być null
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            else if (!LoginHelper.IsCurrentUserAdmin() && !LoginHelper.IsCurrentUserMember())
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "trainings/" + updatedTraining.TrainingId.ToString());

            updatedTraining.Date = updatedTraining.Date.ToUniversalTime();
            var trainingSerialized = JsonConvert.SerializeObject(updatedTraining);


            message.Content = new StringContent(trainingSerialized, System.Text.Encoding.UTF8, "application/json"); //dog serialized id.ToString()
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                var resp = responseMessage.Content.ReadAsStringAsync().Result;
                var id = Int32.Parse(resp);
                return RedirectToAction("Training", new { id });
                //wywolać metodę Dog zamiast zwracać true

            }
            else    // wiadomosc czego się nie udało
            {
                message.Dispose();
                ViewBag.Message = "code:" + responseMessage.StatusCode;
                return View("Error");
            }

        }

    }
}