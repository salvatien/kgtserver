using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
//using kgtwebClient.Models;
using Dogs.ViewModels.Data.Models;
using Newtonsoft.Json;

namespace kgtwebClient.Helpers
{
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