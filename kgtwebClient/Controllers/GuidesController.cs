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

namespace kgtwebClient.Controllers
{
    public class GuidesController : Controller
    {

        //The URL of the WEB API Service
#if DEBUG
        static string url = "http://localhost:12321/api/";
#else
        static string url = "http://kgt.azurewebsites.net/api/";
#endif
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        // get all dogs from db
        public async Task<ActionResult> Index()
        {
            ////client.BaseAddress = new Uri(url);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //HttpResponseMessage responseMessage = await client.GetAsync("guides/");
            //if (responseMessage.IsSuccessStatusCode)
            //{
            //    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
            //    var guides = JsonConvert.DeserializeObject<List<GuideModel>>(responseData);

            //    var guidesList = new GuideListModel
            //    {
            //        ListOfGuides = guides
            //    };

            //    ViewBag.RawData = responseData;

            //    return View(guidesList);
            //}
            //return View();
            var guides = GuideHelpers.GetAllGuides().Result;
            if (guides.ListOfGuides.Any())
                return View(guides);
            return View();
        }

        public async Task<ActionResult> Guide(int id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync("guides/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var guide = JsonConvert.DeserializeObject<GuideModel>(responseData);

                return View(guide);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> AddGuide()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddGuide(GuideModel addedGuide)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* for put $ post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "guides/");
            /*
            var dog = new DogModel
            {
                //DogID = 1,
                Name = addedDog.Name,
                DateOfBirth = addedDog.DateOfBirth,
                Level = addedDog.Level,
                Workmodes = addedDog.Workmodes,
                Notes = addedDog.Notes,
                //change -> add field in form 
                GuideId = 1 //IT DOESNT WORK, IT SHOULD BE A REAL GUIDE, NOW SERVER JUST IGNORES GUIDE AND LEAVES THE OLD ONE UNCHANGED!
            };*/

            var guideSerialized = JsonConvert.SerializeObject(addedGuide);

            message.Content = new StringContent(guideSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return View("Guide", addedGuide);
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                message.Dispose();
                return View(/*error*/);
            }

        }

        public JsonResult DeleteGuide(int? id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "guides/" + id.ToString());
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
        public async Task<ActionResult> UpdateGuide(int id)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.GetAsync("guides/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var guide = JsonConvert.DeserializeObject<GuideModel>(responseData);


                return View(guide);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateGuide(GuideModel updatedGuide)    //? -> może być null
        {
            // add validation function
            /*
            if (!DogHelpers.ValidateUpdateGuide(updatedGuide))
                return false;
            */
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // dla put(update) i post(add):
            //httpmethod.put i httpmethod.post
            //message.Content = new StringContent(***object-json-serialized***, 
            //                                  System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "guides/" + updatedGuide.GuideID.ToString());
            /*var dog = new DogModel
            {
                DogID = updatedDog.DogID,
                Name = updatedDog.Name,
                DateOfBirth = updatedDog.DateOfBirth,
                Level = updatedDog.Level,
                Workmodes = updatedDog.Workmodes,
                Notes = updatedDog.Notes,
                // change in the same way as in add dogs
                GuideId = 1 //IT DOESNT WORK, IT SHOULD BE A REAL GUIDE, NOW SERVER JUST IGNORES GUIDE AND LEAVES THE OLD ONE UNCHANGED!
            };*/

            var guideSerialized = JsonConvert.SerializeObject(updatedGuide);


            message.Content = new StringContent(guideSerialized, System.Text.Encoding.UTF8, "application/json"); //dog serialized id.ToString()
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return View("Guide", updatedGuide);
                //wywolać metodę Dog zamiast zwracać true

            }
            else    // wiadomosc czego się nie udało
            {
                message.Dispose();
                return View(/*error*/);
            }

        }

        //public class SelectListItem
        //{
        //    public int id;
        //    public string text;
        //}

        //public List<SelectListItem> GetAllGuidesIdAndName()
        //{
        //    var guides = GetAllGuides().Result;
        //    return guides.ListOfGuides
        //                 .Select(x => new SelectListItem { id = x.GuideID, text = $"{x.FirstName} {x.LastName}" }).ToList();
        //}

        //private async Task<GuideListModel> GetAllGuides()
        //{
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    HttpResponseMessage responseMessage =  client.GetAsync("guides/").Result;
        //    if (responseMessage.IsSuccessStatusCode)
        //    {
        //        var responseData = responseMessage.Content.ReadAsStringAsync().Result;
        //        var guides = JsonConvert.DeserializeObject<List<GuideModel>>(responseData);

        //        var guidesList = new GuideListModel
        //        {
        //            ListOfGuides = guides
        //        };

        //        return guidesList;
        //    }
        //    return new GuideListModel { ListOfGuides = new List<GuideModel>()};
        //}

    }
}