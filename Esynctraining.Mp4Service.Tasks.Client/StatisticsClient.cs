using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Esynctraining.Mp4Service.Tasks.Client.Dto;
using Esynctraining.WebApi.Client;
using MP4Service.Contract.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class StatisticsClient
    {
        private readonly Uri _baseUrl;


        public StatisticsClient(string baseApiAddress)
        {
            _baseUrl = new Uri(baseApiAddress);
        }


        public async Task<DataPage<ReportInfo>> GetStatistics(string licenseId, DateTime from, DateTime to, string filter,
            int top, int skip)
        {
            string url = string.Format("reports/?licenseId={0}&from={1}&to={2}&$top={3}&$skip={4}",
                licenseId,
                from.ToString("yyyy-MM-dd"),
                to.ToString("yyyy-MM-dd"),
                top,
                skip);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                url += "&$filter=substringof(tolower('" + HttpUtility.UrlEncode(filter.Replace("'", "''")) + "'),tolower(RecordingName)) eq true";
            }

            //using (var request = new HttpRequestMessage(HttpMethod.Get, $"http://{HOST}/statistics?licenseId=none&from={DateTime.Now - TimeSpan.FromHours(1)}&to={DateTime.Now + TimeSpan.FromHours(1)}"))

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<DataPage<ReportInfo>>();
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
