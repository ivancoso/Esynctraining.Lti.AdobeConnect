using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Esynctraining.WebApi.Client;
using MP4Service.Contract.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class LicenseClient
    {
        private readonly Uri _baseUrl;


        public LicenseClient(string baseApiAddress)
        {
            _baseUrl = new Uri(baseApiAddress);
        }


        public async Task<License> GetLicenseStatus(Guid licenseKey)
        {
            if (licenseKey == Guid.Empty)
                throw new ArgumentException("Empty licenseKey key", "licenseKey");

            string url = string.Format("licenses/{0}", licenseKey);
            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<License>();
                }
                else
                {
                    var ex = response.CreateApiException();
                    throw ex;
                }
            }
        }
        

        private static HttpClient BuildClient(Uri baseAddress)
        {
            var client = new HttpClient();
            client.BaseAddress = baseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

    }

}
