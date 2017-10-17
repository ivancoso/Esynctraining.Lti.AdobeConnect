﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.HttpClient
{
    public class HttpClientWrapper
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        protected ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        #region Constructors

        public HttpClientWrapper()
        {
            _httpClient = new System.Net.Http.HttpClient();
        }

        public HttpClientWrapper(TimeSpan timeSpan)
        {
            _httpClient = new System.Net.Http.HttpClient
            {
                Timeout = timeSpan
            };
        }

        public HttpClientWrapper(Uri uri)
        {
            _httpClient = new System.Net.Http.HttpClient
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
            if (!response.IsSuccessStatusCode)
            {
                Logger.Error(response.ToString());
                if (response.Content != null)
                {
                    Logger.Error(await response.Content.ReadAsStringAsync());
                }
                
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
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
