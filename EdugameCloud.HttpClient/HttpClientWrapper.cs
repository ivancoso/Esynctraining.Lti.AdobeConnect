using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EdugameCloud.HttpClient
{
    public class HttpClientWrapper
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        #region Constructors

        public HttpClientWrapper()
        {
            _httpClient = new System.Net.Http.HttpClient(new HttpLoggingHandler(new HttpClientHandler()));
        }

        public HttpClientWrapper(TimeSpan timeout)
        {
            _httpClient = new System.Net.Http.HttpClient(new HttpLoggingHandler(new HttpClientHandler()))
            {
                Timeout = timeout,
            };
        }

        public HttpClientWrapper(Uri uri)
        {
            _httpClient = new System.Net.Http.HttpClient(new HttpLoggingHandler(new HttpClientHandler()))
            {
                BaseAddress = uri
            };
        }

        #endregion Constructors

        #region Public Methods

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
            return response;
        }

        public async Task<string> PostValuesAsync(string url, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(pairs));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostValuesAsync(string url, IEnumerable<KeyValuePair<string, string>> pairs, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(pairs));

            response.EnsureSuccessStatusCode();
            var buffer = await response.Content.ReadAsByteArrayAsync();
            return encoding.GetString(buffer, 0, buffer.Length);
        }
        
        public async Task<string> DownloadStringAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DownloadStringAsync(Uri url)
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UploadJsonStringAsync(string address, string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(address, content);
            return await response.Content.ReadAsStringAsync();
        }

        #endregion Public Methods
    }

}
