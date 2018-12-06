using System;
using System.Collections.Generic;
using System.Linq;
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
#if DEBUG
        static string url = "http://localhost:12321/api/";
#else
        static string url = "http://kgt.azurewebsites.net/api/";
#endif
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        public static List<SelectListItem> GetAllGuidesIdAndName()
        {
            var guides = GetAllGuides().Result;
            return guides.ListOfGuides
                         .Select(x => new SelectListItem { Value = x.GuideID.ToString(), Text = $"{x.FirstName} {x.LastName}" }).ToList();
        }

        public static async Task<GuideListModel> GetAllGuides()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = client.GetAsync("guides/").Result;
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