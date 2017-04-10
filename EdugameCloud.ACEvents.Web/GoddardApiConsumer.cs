using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using EdugameCloud.ACEvents.Web.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EdugameCloud.ACEvents.Web
{
    public interface IGoddardApiConsumer
    {
        Trainer GetTrainer(string trainerId);
    }

    public class GoddardApiConsumer : IGoddardApiConsumer
    {
        private readonly AppSettings _settings;
        private string _apiUrl;

        public GoddardApiConsumer(IOptionsSnapshot<AppSettings> appSettings)
        {
            _settings = appSettings.Value;
            _apiUrl = _settings.GoddardApi;
        }

        private T GetApiCall<T>(string url)
        {
            string reply;
            using (var httpClient = new HttpClient())
            {
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                httpClient.DefaultRequestHeaders.Accept.Add(contentType);
                reply = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(url))).Result.Content.ReadAsStringAsync().Result;
            }
            var entity = JsonConvert.DeserializeObject<T>(reply);
            return entity;
        }

        public Trainer GetTrainer(string trainerId)
        {
            var trainer = GetApiCall<Trainer>(_apiUrl + $"/trainer/{trainerId}");
            return trainer;
        }

        

        //public CoreKnowledgeArea GetCoreKnowledgeArea(string courseId, string stateCode)
        //{
            
        //}
    }
}