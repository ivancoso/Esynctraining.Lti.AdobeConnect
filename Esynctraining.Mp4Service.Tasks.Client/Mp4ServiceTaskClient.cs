using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class Mp4ServiceTaskClient
    {
        public async Task<DataTask> Convert(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            using (var client = BuildClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("task", task);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<DataTask>();
                }
                else
                {
                    // TODO: add some extra logging
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
        }

        public async Task<DataTask> GetStatus(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            string url = string.Format("task?licenseId={0}&scoId={1}", task.LicenseId, task.ScoId);

            using (var client = BuildClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<DataTask>();
                }
                else
                {
                    // TODO: add some extra logging
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
        }


        private static HttpClient BuildClient()
        {
            var client = new HttpClient();
            // TODO: config
            client.BaseAddress = new Uri("http://192.168.10.211/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

    }

}
