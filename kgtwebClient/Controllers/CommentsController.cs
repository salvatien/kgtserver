using Dogs.ViewModels.Data.Models;
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
    //this controller is used only by AJAX calls - it only gets data, doesnt render any views - all methods return JsonResult
    public class CommentsController : Controller
    {
        //The URL of the WEB API Service
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        [HttpGet]
        public JsonResult GetTrainingCommentsByTrainingId(int trainingId)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = client.GetAsync("comments/TrainingCommentsByTrainingId/" + trainingId).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var comments = JsonConvert.DeserializeObject<List<CommentModel>>(responseData);
                return Json(new { success = true, data = comments });
            }
            return Json(new { success = false, errorCode = responseMessage.StatusCode });
        }


        //To be called with AJAX
        [HttpPost]
        public JsonResult AddTrainingComment(CommentModel addedTrainingComment)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* for put $ post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "comments/trainingcomment");

            var commentSerialized = JsonConvert.SerializeObject(addedTrainingComment);

            message.Content = new StringContent(commentSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return Json(new { success = true, id= Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
            }
            else    // msg why not ok
            {
                message.Dispose();
                return Json(new { success = false, errorCode = responseMessage.StatusCode });
            }

        }

        //to be called with AJAX
        public JsonResult DeleteTrainingComment(int? id)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "comments/trainingcomment/" + id.ToString());
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



        [HttpGet]
        public JsonResult GetDogTrainingCommentsByDogIdAndTrainingId(int dogId, int trainingId)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = 
                client.GetAsync($"comments/DogTrainingCommentsByDogIdAndTrainingId?trainingId={trainingId}&dogId={dogId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var comments = JsonConvert.DeserializeObject<List<CommentModel>>(responseData);
                return Json(new { success = true, data = comments });
            }
            return Json(new { success = false, errorCode = responseMessage.StatusCode });
        }


        //To be called with AJAX
        [HttpPost]
        public JsonResult AddDogTrainingComment(CommentModel addedDogTrainingComment)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* for put $ post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "comments/dogtrainingcomment");

            var commentSerialized = JsonConvert.SerializeObject(addedDogTrainingComment);

            message.Content = new StringContent(commentSerialized, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.SendAsync(message).Result; // await client.SendAsync(message)
            if (responseMessage.IsSuccessStatusCode)    //200 OK
            {
                //display info
                message.Dispose();
                return Json(new { success = true, id = Int32.Parse(responseMessage.Content.ReadAsStringAsync().Result) });
            }
            else    // msg why not ok
            {
                message.Dispose();
                return Json(new { success = false, errorCode = responseMessage.StatusCode });
            }

        }

        //to be called with AJAX
        public JsonResult DeleteDogTrainingComment(int? id)
        {
            if (!LoginHelper.IsAuthenticated())
                return Json(new { success = false, errorCode = "403" });
            //client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            /* dla put i post:
            httpmethod.put i httpmethod.post
            message.Content = new StringContent(***object-json-serialized***, 
                                                System.Text.Encoding.UTF8, "application/json");
             */
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + "comments/dogtrainingcomment/" + id.ToString());
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
    }
}