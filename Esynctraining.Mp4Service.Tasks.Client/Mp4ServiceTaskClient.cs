using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class Mp4ServiceTaskClient
    {
        private readonly Uri _baseUrl;


        public Mp4ServiceTaskClient(string baseApiAddress)
        {
            _baseUrl = new Uri(baseApiAddress);
        }


        public async Task<MP4Service.Contract.Client.DataTask> Convert(MP4Service.Contract.Client.TaskParam task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("task", task);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MP4Service.Contract.Client.DataTask>();
                }
                else
                {
                    // TODO: add some extra logging
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
        }

        public async Task<MP4Service.Contract.Client.DataTask> GetStatus(MP4Service.Contract.Client.TaskParam task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            string url = string.Format("task/{0}/{1}", task.LicenseId, task.ScoId);

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var rest = response.Content.ReadAsAsync<MP4Service.Contract.Client.DataTask>().Result;
                    return rest;
                }
                else
                {
                    // TODO: add some extra logging
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
        }

        public async Task<MP4Service.Contract.Client.License> GetLicense(Guid licenseId)
        {
            if (licenseId == Guid.Empty)
                throw new ArgumentException("Empty licenseId key", "licenseId");

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(string.Format("license/{0}", licenseId));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MP4Service.Contract.Client.License>();
                }
                else
                {
                    // TODO: add some extra logging
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
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
