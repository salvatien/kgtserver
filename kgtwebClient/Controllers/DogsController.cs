using kgtwebClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using kgtwebClient.Helpers;

namespace kgtwebClient.Controllers
{
    public class DogsController : Controller
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
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
        public async Task<ActionResult> AddDog()
        {
            return View();
        }

        [HttpPost]
        public JsonResult /*Task<ActionResult>*/ AddDog(DogModel addedDog)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* for put $ post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "dogs/");
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

            var dogSerialized = JsonConvert.SerializeObject(addedDog);

            message.Content = new StringContent(dogSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return Json(new { success = true , responseMessage.Content});
                //return View("Dog", responseMessage.Content);
            }
            else    // msg why not ok
            {
                message.Dispose();
                return Json(false);
            }

        }

        public JsonResult DeleteDog(int? id)
        {
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress +"dogs/" + id.ToString());
            message.Content = new StringContent(id.ToString(), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return Json(new { success = true, id = id.ToString() } );
            }
            else    // wiadomosc czego się nie udałos
            {
                message.Dispose();
                return Json(false);
            }

        }

        //TODO metody UpdateDog(ta niżej) i Dog robią to samo -> wyrzucić środek do innej metody i wywoływać ją sobie wewnątrz
        [HttpGet]
        public async Task<ActionResult> UpdateDog(int id) {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
        public bool UpdateDog(DogModel updatedDog)    //? -> może być null
        {
            if (!DogHelpers.ValidateUpdateDog(updatedDog))
                return false;

            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // dla put(update) i post(add):
            //httpmethod.put i httpmethod.post
            //message.Content = new StringContent(***object-json-serialized***, 
              //                                  System.Text.Encoding.UTF8, "application/json");
             
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "dogs/" + updatedDog.DogID.ToString());
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

            var dogSerialized = JsonConvert.SerializeObject(updatedDog);
            

            message.Content = new StringContent(dogSerialized, System.Text.Encoding.UTF8, "application/json"); //dog serialized id.ToString()
            HttpResponseMessage responseMessage = client.SendAsync(message).Result;
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //wyswietlić informację
                message.Dispose();
                return true;
                //wywolać metodę Dog zamiast zwracać true
                
            }
            else    // wiadomosc czego się nie udało
            {
                message.Dispose();
                return false;
            }

        }
    
    }
}