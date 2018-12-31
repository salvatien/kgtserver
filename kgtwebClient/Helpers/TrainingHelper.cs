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
    public class TrainingEqualityComparer : IEqualityComparer<TrainingModel>
    {
        bool IEqualityComparer<TrainingModel>.Equals(TrainingModel x, TrainingModel y)
        {
            return x.TrainingId.Equals(y.TrainingId);
        }


        int IEqualityComparer<TrainingModel>.GetHashCode(TrainingModel obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.TrainingId.GetHashCode();
        }
    }

public class TrainingHelpers
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        
        public static List<SelectListItem> GetAllTrainingsExceptOneDog(int dogId)
        {
            var allDogTrainings = GetAllTrainings().Result;
            var dogTrainings = GetTrainingsByDogId(dogId).Result;
            var remainingTrainings = allDogTrainings.Except(dogTrainings, new TrainingEqualityComparer());

            return remainingTrainings.Select(x => new SelectListItem { Value = x.TrainingId.ToString(),
                                                                       Text = $"{x.GeneralLocation} {x.LocationDetails} {x.Date}" })
                                     .ToList();
        }

        public static async Task<List<TrainingModel>> GetTrainingsByDogId(int dogId)
        {
             //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync($"dogtrainings/GetAllByDogId?dogId={dogId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogTrainings = JsonConvert.DeserializeObject<List<DogTrainingModel>>(responseData);

                return dogTrainings.Select(x => x.Training).ToList();

            }
            return new List<TrainingModel>();
        }

        public static async Task<List<TrainingModel>> GetAllTrainings()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            
            HttpResponseMessage responseMessage = client.GetAsync("Trainings/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var trainings = JsonConvert.DeserializeObject<List<TrainingModel>>(responseData);

                return trainings;
            }
            return new List<TrainingModel>();
        }
    }
}