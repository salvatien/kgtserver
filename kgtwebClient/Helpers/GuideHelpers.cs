using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dogs.ViewModels.Data.Models;
using Newtonsoft.Json;

namespace kgtwebClient.Helpers
{
    public class GuideHelpers
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        public static List<SelectListItem> GetAllGuidesIdAndName()
        {
            var guides = GetAllGuides().Result;
            return guides.ListOfGuides
                         .Select(x => new SelectListItem { Value = x.GuideId.ToString(), Text = $"{x.FirstName} {x.LastName}" }).ToList();
        }

        public static async Task<GuideListModel> GetAllGuides()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //not necessary as get all guides is not blocked on the server, but added here just in case we want to change it
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //client.DefaultRequestHeaders.ExpectContinue = false;
            //client.DefaultRequestHeaders.Add("Connection", "close");


            HttpResponseMessage responseMessage = client.GetAsync("Guides/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var guides = JsonConvert.DeserializeObject<List<GuideModel>>(responseData);

                var guidesList = new GuideListModel
                {
                    ListOfGuides = guides
                };

                return guidesList;
            }
            return new GuideListModel { ListOfGuides = new List<GuideModel>() };
        }
    }
}