using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.WebApi.Client;
using MP4Service.Contract.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class LicenseClient
    {
        private readonly Uri _baseUrl;
        private readonly ILogger _logger;


        public LicenseClient(string baseApiAddress, ILogger logger)
        {
            _baseUrl = new Uri(baseApiAddress);
            _logger = logger;
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
                    try
                    {
                        return await response.Content.ReadAsAsync<License>();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(string.Format("Failed to parse JSON (response: {0}; url:{1})",
                            await response.Content.ReadAsStringAsync(),
                            url),
                            ex);
                        throw;
                    }
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
