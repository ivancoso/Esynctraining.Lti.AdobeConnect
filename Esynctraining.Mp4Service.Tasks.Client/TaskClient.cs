using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.WebApi.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class TaskClient
    {
        private readonly Uri _baseUrl;
        private readonly ILogger _logger;


        public TaskClient(string baseApiAddress, ILogger logger)
        {
            _baseUrl = new Uri(baseApiAddress);
            _logger = logger;
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
                    return await GetTask(response);
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
                    return await GetTask(response);
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
                string url = string.Format("licenses/{0}", licenseId);
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<MP4Service.Contract.Client.License>();
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

        private async Task<MP4Service.Contract.Client.DataTask> GetTask(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsAsync<MP4Service.Contract.Client.DataTask>();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Failed to parse task JSON (response: {0})",
                    await response.Content.ReadAsStringAsync()),
                    ex);
                throw;
            }
        }

    }

}
