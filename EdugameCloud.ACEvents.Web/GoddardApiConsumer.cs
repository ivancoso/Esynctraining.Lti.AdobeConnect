using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using EdugameCloud.ACEvents.Web.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EdugameCloud.ACEvents.Web
{
    public class GoddardApiConsumer : IGoddardApiConsumer
    {
        private readonly AppSettings _settings;
        private readonly ILogger _logger;
        private string _apiUrl;

        public GoddardApiConsumer(IOptionsSnapshot<AppSettings> appSettings, ILoggerFactory loggerFactory)
        {
            _settings = appSettings.Value;
            _apiUrl = _settings.GoddardApi;
            _logger = loggerFactory.CreateLogger<GoddardApiConsumer>();
        }

        private T GetApiCall<T>(string url) where T:class
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError(1000, e, e.Message);
                return (T)null;
            }
        }

        public Trainer GetTrainer(string trainerId)
        {
            var entity = GetApiCall<Trainer>(_apiUrl + $"/trainer/{trainerId}");
            return entity;
        }

        public Course GetCourse(string courseId)
        {
            var entity = GetApiCall<Course>(_apiUrl + $"/course/{courseId}");
            return entity;
        }

        public CoreKnowledgeArea GetCoreKnowledgeArea(string courseId, string stateCode)
        {
            var entity = GetApiCall<CoreKnowledgeArea>(_apiUrl + $"/area/{courseId}/{stateCode}");
            return entity;
        }

        public StateTrainerNumber GetStateTrainerNumber(string trainerId, string stateCode)
        {
            var entity = GetApiCall<StateTrainerNumber>(_apiUrl + $"/statetrainernumber/{trainerId}/{stateCode}");
            return entity;
        }

        public StateTrainerCourseNumber GetStateTrainerCourseNumber(string courseId, string stateCode, string trainerId)
        {
            var entity = GetApiCall<StateTrainerCourseNumber>(_apiUrl + $"/statetrainercoursenumber/{courseId}/{stateCode}/{trainerId}");
            return entity;
        }

        public EventStateCourseNumber GetEventStateCourseNumber(string eventId, string stateCode)
        {
            var entity = GetApiCall<EventStateCourseNumber>(_apiUrl + $"/eventstatecoursenumber/{eventId}/{stateCode}");
            return entity;
        }
    }
}