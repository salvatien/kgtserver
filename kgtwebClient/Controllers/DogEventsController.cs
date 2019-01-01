using Dogs.ViewModels.Data.Models;
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
    public class DogEventsController : Controller
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        public async Task<ActionResult> Index(int dogId)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync($"dogEvents/GetAllByDogId?dogId={dogId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogEvents = JsonConvert.DeserializeObject<List<DogEventModel>>(responseData);


                ViewBag.RawData = responseData;

                return View(dogEvents);
            }
            return View();
        }

        public async Task<ActionResult> DogEvent(int eventId, int dogId)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync($"dogEvents/DogEvent?dogId={dogId}&eventId={eventId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogEvent = JsonConvert.DeserializeObject<DogEventModel>(responseData);


                ViewBag.RawData = responseData;

                return View(dogEvent);
            }
            return View();
        }

        public JsonResult DeleteDogEvent(int? dogId, int? eventId)
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
                return Json(false);
            }

        }

        [HttpGet]
        public ActionResult AddDogEvent(int dogId)
        {
            return View(new DogEventModel { DogId = dogId });
        }


        [HttpPost]
        public ActionResult AddDogEvent(DogEventModel addedDogEvent)
        {

            //add dogtraining
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "dogevents/");

            var dogEventSerialized = JsonConvert.SerializeObject(addedDogEvent);

            message.Content = new StringContent(dogEventSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var definition = new { DogId = "", EventId = "" };
                var ids = JsonConvert.DeserializeAnonymousType(responseData, definition);
                return RedirectToAction("Index", new { dogId = ids.DogId });
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                message.Dispose();
                ViewBag.Message = responseMessage.StatusCode;
                return View("Error");
            }
        }
    }
}