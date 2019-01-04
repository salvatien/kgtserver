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
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        // get all guides from db
        public async Task<ActionResult> Index()
        {
            
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var guides = GuideHelpers.GetAllGuides().Result;
            if (guides.ListOfGuides.Any())
                return View(guides);
            return View();
        }

        public async Task<ActionResult> Guide(int id)
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("guides/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var guide = JsonConvert.DeserializeObject<GuideModel>(responseData);

                return View(guide);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        } 

        public JsonResult DeleteGuide(int? id)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = 403 });
            else if (!LoginHelper.IsCurrentUserAdmin() && LoginHelper.GetCurrentUserId() != id)
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
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            
            else if (!LoginHelper.IsCurrentUserAdmin() && LoginHelper.GetCurrentUserId() != id )
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            HttpResponseMessage responseMessage = await client.GetAsync("guides/" + id.ToString());
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var guide = JsonConvert.DeserializeObject<GuideModel>(responseData);


                return View(guide);
            }
            ViewBag.Message = "Kod błędu: " + responseMessage.StatusCode;
            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateGuide(GuideModel updatedGuide)    //? -> może być null
        {
            if (!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            else if (!LoginHelper.IsCurrentUserAdmin() && LoginHelper.GetCurrentUserId() != updatedGuide.GuideId)
                return RedirectToAction("Error", "Home", new { error = "Nie masz wystarczających uprawnień by zmieniać te dane" });

            // add validation function
            /*
            if (!DogHelpers.ValidateUpdateGuide(updatedGuide))
                return false;
            */
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            // dla put(update) i post(add):
            //httpmethod.put i httpmethod.post
            //message.Content = new StringContent(***object-json-serialized***, 
            //                                  System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress + "guides/" + updatedGuide.GuideId.ToString());
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
                return RedirectToAction("Guide", new { id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
                //wywolać metodę Dog zamiast zwracać true

            }
            else    // wiadomosc czego się nie udało
            {
                message.Dispose();
                ViewBag.Message = "Kod błędu:" + responseMessage.StatusCode;
                return View("Error");
            }

        }

    }
}