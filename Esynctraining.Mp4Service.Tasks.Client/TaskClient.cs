using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Esynctraining.WebApi.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class TaskClient
    {
        private readonly Uri _baseUrl;


        public TaskClient(string baseApiAddress)
        {
            _baseUrl = new Uri(baseApiAddress);
        }


        public async Task<MP4Service.Contract.Client.DataTask> Convert(MP4Service.Contract.Client.TaskParam task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("tasks", task);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MP4Service.Contract.Client.DataTask>();
                }
                else
                {
                    // TODO: add some extra logging
                    var ex = response.CreateApiException();
                    throw ex;
                }
            }
        }

        public async Task<MP4Service.Contract.Client.DataTask> GetStatus(MP4Service.Contract.Client.TaskParam task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            string url = string.Format("tasks/{0}/{1}", task.LicenseId, task.ScoId);

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MP4Service.Contract.Client.DataTask>();
                }
                else
                {
                    // TODO: add some extra logging
                    var ex = response.CreateApiException();
                    throw ex;
                }
            }
        }

        public async Task<MP4Service.Contract.Client.License> GetLicense(Guid licenseId)
        {
            if (licenseId == Guid.Empty)
                throw new ArgumentException("Empty licenseId key", "licenseId");

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(string.Format("licenses/{0}", licenseId)).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MP4Service.Contract.Client.License>();
                }
                else
                {
                    // TODO: add some extra logging
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
