using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EdugameCloud.HttpClient
{
    public class HttpClientWrapper
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        #region Constructors

        public HttpClientWrapper()
        {
            _httpClient = new System.Net.Http.HttpClient();
        }

        #endregion Constructors

        #region Public Methods

        public string PostValues(string url, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            var response = _httpClient.PostAsync(url, new FormUrlEncodedContent(pairs)).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string DownloadString(string url)
        {
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
            HttpContent content = response.Content;
            return content.ReadAsStringAsync().Result;
        }

        public string UploadJsonString(string address, string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            return _httpClient.PostAsync(address, content).Result.Content.ReadAsStringAsync().Result;
        }

        #endregion Public Methods
    }
}
