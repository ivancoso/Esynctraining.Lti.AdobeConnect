﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EdugameCloud.Lti.API.Schoology;

namespace EdugameCloud.Lti.Schoology
{
    public class SchoologyRestApiClient : ISchoologyRestApiClient
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly HttpClient _client;


        public SchoologyRestApiClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.schoology.com/v1/"),
            };
        }


        public async Task<T> GetRestCall<T>(string clientId, string clientSecret, string relativeUrl)
        {
            long secondsSince1970 = (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;

            var nameValueCollection = new NameValueCollection
            {
                { "realm", "Schoology API" },
                { "oauth_consumer_key", clientId },
                { "oauth_token", "" },
                { "oauth_nonce", Guid.NewGuid().ToString() },
                { "oauth_timestamp", secondsSince1970.ToString() },
                { "oauth_signature_method", "PLAINTEXT" },
                { "oauth_version", "1.0" },

                { "oauth_signature", clientSecret + "%26" }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

            var header = new StringBuilder();
            foreach (string key in nameValueCollection.AllKeys)
                header.AppendFormat("{0}=\"{1}\", ", key, nameValueCollection[key]);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header.ToString().TrimEnd(',', ' '));
            var response = await _client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }
        
    }

}
