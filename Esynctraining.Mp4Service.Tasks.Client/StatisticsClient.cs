using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Esynctraining.Core.Logging;
using Esynctraining.Mp4Service.Tasks.Client.Dto;
using Esynctraining.WebApi.Client;
using MP4Service.Contract.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class StatisticsClient
    {
        private readonly Uri _baseUrl;
        private readonly ILogger _logger;


        public StatisticsClient(string baseApiAddress, ILogger logger)
        {
            _baseUrl = new Uri(baseApiAddress);
            _logger = logger;
        }


        public async Task<DataPage<ReportInfo>> GetStatistics(Guid licenseId, DateTime from, DateTime to, string filter,
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

            using (var client = BuildClient(_baseUrl))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<DataPage<ReportInfo>>();
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

    }

}
