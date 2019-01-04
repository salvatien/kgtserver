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

namespace kgtwebClient.Helpers
{
    public class EventEqualityComparer : IEqualityComparer<EventModel>
    {
        bool IEqualityComparer<EventModel>.Equals(EventModel x, EventModel y)
        {
            return x.EventId.Equals(y.EventId);
        }


        int IEqualityComparer<EventModel>.GetHashCode(EventModel obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.EventId.GetHashCode();
        }
    }

    public class EventHelper
    {

        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        public static List<SelectListItem> GetAllEventsExceptOneDog(int dogId)
        {
            var allDogEvents = GetAllEvents();
            var dogEvents = GetEventsByDogId(dogId).Result;
            var remainingEvents = allDogEvents.Except(dogEvents, new EventEqualityComparer());

            //select only this event, which has IsCommercialTraining flag set to true - only this events may be dogEvents
            remainingEvents = remainingEvents.Where(x => x.IsCommercialTraining == true).ToList();

            return remainingEvents.Select(x => new SelectListItem
            {
                Value = x.EventId.ToString(),
                Text = $"{x.Title},{x.StreetOrLocation}, {x.Date}"
            })
                                     .ToList();
        }

        public static async Task<List<EventModel>> GetEventsByDogId(int dogId)
        {
            //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync($"dogEvents/GetAllByDogId?dogId={dogId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogEvents = JsonConvert.DeserializeObject<List<DogEventModel>>(responseData);

                return dogEvents.Select(x => x.Event).ToList();

            }
            return new List<EventModel>();
        }

        public static List<EventModel> GetAllEvents()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync("Events/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var Events = JsonConvert.DeserializeObject<List<EventModel>>(responseData);

                return Events;
            }
            return new List<EventModel>();
        }
    }
}