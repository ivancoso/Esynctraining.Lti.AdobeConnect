using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.Extensions.Caching.Distributed;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class UserCacheUpdater
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public UserCacheUpdater(ILogger logger, IJsonSerializer jsonSerializer, IDistributedCache cache)
        {
            _jsonSerializer = jsonSerializer;
            _cache = cache;
            _logger = logger;
        }

        public async Task UpdateUsers(string zoomKey, ZoomApiWrapper apiWrapper)
        {
            var cacheKey = "Zoom.Users." + zoomKey;
            var sw = Stopwatch.StartNew();
            var users = GetUsersFromApi(UserStatus.Active, apiWrapper);
            var json = _jsonSerializer.JsonSerialize(users);
            var cacheData = Encoding.UTF8.GetBytes(json);
            var cacheEntryOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // todo: better approach
            };

            await _cache.SetAsync(cacheKey, cacheData, cacheEntryOption);
            sw.Stop();
            _logger.Info($"[UpdateCache:{zoomKey}] Time: {sw.Elapsed}, UsersCount:{users.Count}");
        }


        public List<User> GetUsersFromApi(UserStatus status, ZoomApiWrapper apiWrapper)
        {
            var users = new List<User>();
            var pageNumber = 1;
            var pageSize = 300;
            var totalRecords = 0;
            do
            {
                var page = apiWrapper.GetUsers(status, pageSize: pageSize, pageNumber: pageNumber);
                users.AddRange(page.Users);
                totalRecords = page.TotalRecords;
                pageNumber++;

            } while (pageSize * (pageNumber - 1) < totalRecords);

            return users;
        }
    }
}