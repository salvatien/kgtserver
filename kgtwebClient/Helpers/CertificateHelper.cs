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
    public class CertificateEqualityComparer : IEqualityComparer<CertificateModel>
    {
        bool IEqualityComparer<CertificateModel>.Equals(CertificateModel x, CertificateModel y)
        {
            return x.CertificateId.Equals(y.CertificateId);
        }


        int IEqualityComparer<CertificateModel>.GetHashCode(CertificateModel obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.CertificateId.GetHashCode();
        }
    }

    public class CertificateHelper
    {

        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };


        public static List<SelectListItem> GetAllCertificatesExceptOneDog(int dogId)
        {
            var allDogCertificates = GetAllCertificates().Result;
            var dogCertificates = GetCertificatesByDogId(dogId).Result;
            var remainingCertificates = allDogCertificates.Except(dogCertificates, new CertificateEqualityComparer());

            return remainingCertificates.Select(x => new SelectListItem
            {
                Value = x.CertificateId.ToString(),
                Text = $"{x.Name}, poziom: {x.Level}"
            })
                                     .ToList();
        }

        public static async Task<List<CertificateModel>> GetCertificatesByDogId(int dogId)
        {
            //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync($"dogCertificates/GetAllByDogId?dogId={dogId}").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var dogCertificates = JsonConvert.DeserializeObject<List<DogCertificateModel>>(responseData);

                return dogCertificates.Select(x => x.Certificate).ToList();

            }
            return new List<CertificateModel>();
        }

        public static async Task<List<DogModel>> GetDogsByCertificateId(int certificateId)
        {
            //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
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

        public static async Task<List<CertificateModel>> GetAllCertificates()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", LoginHelper.GetToken());
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = client.GetAsync("Certificates/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var certificates = JsonConvert.DeserializeObject<List<CertificateModel>>(responseData);

                return certificates;
            }
            return new List<CertificateModel>();
        }
    }
}