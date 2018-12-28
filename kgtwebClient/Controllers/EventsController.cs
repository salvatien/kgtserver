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

namespace kgtwebClient.Controllers
{
    public class EventsController : Controller
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        // get all dogs from db
        public async Task<ActionResult> Index()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            HttpResponseMessage responseMessage = await client.GetAsync("Events/");
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var events = JsonConvert.DeserializeObject<List<EventModel>>(responseData);

                if (events.Any())
                    return View(events);
                else
                    return View();
            }
            ViewBag.Message = "code:" + responseMessage.StatusCode;
            return View("Error");
        }

        public async Task<ActionResult> Event(int id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync("events/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var oneEvent = JsonConvert.DeserializeObject<EventModel>(responseData);

                return View(oneEvent);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> AddEvent()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddEvent(EventModel addedEvent)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "events/");


            var eventSerialized = JsonConvert.SerializeObject(addedEvent);

            message.Content = new StringContent(eventSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await client.SendAsync(message);
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return RedirectToAction("Event", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
            }
            else    // msg why not ok
            {
                message.Dispose();
                ViewBag.Message = "code:" + responseMessage.StatusCode;
                return View("Error");
            }

        }

        public JsonResult DeleteEvent(int? id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "events/" + id.ToString());
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
        public async Task<ActionResult> UpdateEvent(int id)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync("events/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var oneEvent = JsonConvert.DeserializeObject<EventModel>(responseData);


                return View(oneEvent);
            }
            ViewBag.Message = "code:" + responseMessage.StatusCode;
            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateEvent(EventModel updatedEvent)    //? -> może być null
        {

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "events/" + updatedEvent.EventId.ToString());


            var eventSerialized = JsonConvert.SerializeObject(updatedEvent);


            message.Content = new StringContent(eventSerialized, System.Text.Encoding.UTF8, "application/json"); 
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                var resp = responseMessage.Content.ReadAsStringAsync().Result;
                var id = Int32.Parse(resp);
                return RedirectToAction("Event", new { id });

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