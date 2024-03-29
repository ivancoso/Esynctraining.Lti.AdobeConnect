﻿using System;
using System.Collections.Generic;
using System.Net;
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
            var handler = new HttpClientHandler()
            {
                //https://stackoverflow.com/questions/46400797/httpclienthandler-throwing-platformnotsupportedexception
                //SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Ssl3,
                //ServerCertificateCustomValidationCallback = delegate { return true; },
            };
            _httpClient = new System.Net.Http.HttpClient(new HttpLoggingHandler(handler));
        }

        public HttpClientWrapper(TimeSpan timeout, CookieContainer cookies = null, bool allowAutoRedirect = true)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = allowAutoRedirect

                //https://stackoverflow.com/questions/46400797/httpclienthandler-throwing-platformnotsupportedexception
                //SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Ssl3,
                //ServerCertificateCustomValidationCallback = delegate { return true; },
            };

            if (cookies != null)
            {
                handler.CookieContainer = cookies;
            }

            _httpClient = new System.Net.Http.HttpClient(new HttpLoggingHandler(handler))
            {
                Timeout = timeout,
            };
        }

        public HttpClientWrapper(Uri baseAddress)
        {
            var handler = new HttpClientHandler()
            {
                //https://stackoverflow.com/questions/46400797/httpclienthandler-throwing-platformnotsupportedexception
                //SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Ssl3,
                //ServerCertificateCustomValidationCallback = delegate { return true; },
            };
            _httpClient = new System.Net.Http.HttpClient(new HttpLoggingHandler(handler))
            {
                BaseAddress = baseAddress,
            };
        }

        #endregion Constructors

        #region Public Methods

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            try
            {
                PreCall();

                HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
                return response;
            }
            finally
            {
                PostCall();
            }
        }

        public async Task<string> PostValuesAsync(string url, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            try
            {
                PreCall();

                var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(pairs));

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                PostCall();
            }
        }

        public async Task<string> PostValuesAsync(string url, IEnumerable<KeyValuePair<string, string>> pairs, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            try
            {
                PreCall();

                var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(pairs));

                response.EnsureSuccessStatusCode();
                var buffer = await response.Content.ReadAsByteArrayAsync();
                return encoding.GetString(buffer, 0, buffer.Length);
            }
            finally
            {
                PostCall();
            }
        }
        
        public async Task<string> DownloadStringAsync(string url)
        {
            try
            {
                PreCall();

                var response = await _httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                PostCall();
            }
        }

        public async Task<string> DownloadStringAsync(Uri url)
        {
            try
            {
                PreCall();

                var response = await _httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                PostCall();
            }
        }

        public async Task<string> UploadJsonStringAsync(string address, string data)
        {
            try
            {
                PreCall();

                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(address, content);
                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                PostCall();
            }
        }

        #endregion Public Methods

        private static void PreCall()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
        }

        private static void PostCall()
        {
            // NOTE: do nothing for now
            // Can restore ServicePointManager.SecurityProtocol here
        }

    }

}
