using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
//using kgtwebClient.Models;
using Dogs.ViewModels.Data.Models;
using Newtonsoft.Json;

namespace kgtwebClient.Helpers
{

    public class DogEqualityComparer : IEqualityComparer<DogModel>
    {
        bool IEqualityComparer<DogModel>.Equals(DogModel x, DogModel y)
        {
            return x.DogId.Equals(y.DogId);
        }

        int IEqualityComparer<DogModel>.GetHashCode(DogModel obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.DogId.GetHashCode();
        }
    }
    // pobrać 
    // pobrc wszystkie psy po id treningu
    public class DogHelpers
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        public static List<SelectListItem> GetAllDogsIdAndName()
        {
            var dogs = GetAllDogs();
            return dogs.Select(x => new SelectListItem { Value = x.DogId.ToString(), Text = x.Name }).ToList();
        }

        public static List<DogModel> GetAllDogs()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //client.DefaultRequestHeaders.ExpectContinue = false;
            //client.DefaultRequestHeaders.Add("Connection", "close");


            HttpResponseMessage responseMessage = client.GetAsync("Dogs/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogs = JsonConvert.DeserializeObject<List<DogModel>>(responseData);
                return dogs;
            }
            return new List<DogModel>();
        }

        public static List<DogModel> GetDogsByTrainingId(int trainingId)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //client.DefaultRequestHeaders.ExpectContinue = false;
            //client.DefaultRequestHeaders.Add("Connection", "close");


            HttpResponseMessage responseMessage = client.GetAsync($"DogTrainings/GetAllByTrainingId?trainingId={trainingId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogTrainings = JsonConvert.DeserializeObject<List<DogTrainingModel>>(responseData);
                return dogTrainings.Select(x=>x.Dog).ToList();
            }
            return new List<DogModel>();
        }

        public static List<SelectListItem> GetDogsNotInTraining(int trainingId)
        {
            var allDogs = GetAllDogs();

            var dogsInTraining = GetDogsByTrainingId(trainingId);
            
            var remainingDogs = allDogs.Except(dogsInTraining, new DogEqualityComparer());

            return remainingDogs.Select(x => new SelectListItem
            {
                Value = x.DogId.ToString(),
                Text = $"{x.Name}, {x.GuideIdAndName.Name}"
            })
                                     .ToList();

        }

        public static List<DogModel> GetDogsByEventId(int eventId)
        {
            //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync($"dogEvents/GetAllByEventId?eventId={eventId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogEvents = JsonConvert.DeserializeObject<List<DogEventModel>>(responseData);

                return dogEvents.Select(x => x.Dog).ToList();

            }
            return new List<DogModel>();
        }

        public static List<SelectListItem> GetDogsNotInEvent(int eventId)
        {
            var allDogs = GetAllDogs();
            var dogsInEvent = GetDogsByEventId(eventId);

            var remainingDogs = allDogs.Except(dogsInEvent, new DogEqualityComparer());

            return remainingDogs.Select(x => new SelectListItem
            {
                Value = x.DogId.ToString(),
                Text = $"{x.Name}, {x.GuideIdAndName.Name}"
            })
                                     .ToList();

        }

        public static List<DogModel> GetDogsByCertificateId(int certificateId)
        {
            //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync($"dogCertificates/GetAllByCertificateId?certificateId={certificateId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogCertificates = JsonConvert.DeserializeObject<List<DogCertificateModel>>(responseData);

                return dogCertificates.Select(x => x.Dog).ToList();

            }
            return new List<DogModel>();
        }

        public static List<SelectListItem> GetDogsNotHavingCertificate(int certificateId)
        {
            var allDogs = GetAllDogs();
            var dogsHavingCertificate = GetDogsByCertificateId(certificateId);

            var remainingDogs = allDogs.Except(dogsHavingCertificate, new DogEqualityComparer());

            return remainingDogs.Select(x => new SelectListItem
            {
                Value = x.DogId.ToString(),
                Text = $"{x.Name}, {x.GuideIdAndName.Name}"
            })
                                     .ToList();

        }
        
        static public bool ValidateUpdateDog(DogModel updatedDog)
        {
            if (!ValidateDogName(updatedDog.Name) || !ValidateDogDateOfBirth(updatedDog.DateOfBirth))
                return false;           
            return true;
        }
        
        static private bool ValidateDogName(string name)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
                return false;
            return true;
        }

        static private bool ValidateDogDateOfBirth(DateTime date)
        {
            if (date > DateTime.Now)
                return false;
            return true;
        }
    }
}