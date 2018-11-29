using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.Extensions.Caching.Distributed;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class ZoomUserService
    {
        private readonly ZoomApiWrapper _zoomApi;
        private readonly IDistributedCache _cache;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly IJsonDeserializer _jsonDeserializer;
        private readonly dynamic _settings;
        private readonly UserCacheUpdater _cacheUpdater;
        private readonly ILogger _logger;

        public ZoomUserService(ZoomApiWrapper zoomApi, IDistributedCache cache, ILmsLicenseAccessor licenseAccessor, 
            ApplicationSettingsProvider settings, IJsonDeserializer jsonDeserializer, UserCacheUpdater cacheUpdater, ILogger logger)
        {
            _zoomApi = zoomApi ?? throw new ArgumentNullException(nameof(zoomApi));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _settings = settings;
            _jsonDeserializer = jsonDeserializer;
            _cacheUpdater = cacheUpdater;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetActiveUsers(LmsLicenseDto licenseDto = null)
        {
            var license = licenseDto ?? await _licenseAccessor.GetLicense();
            var cacheKey = "Zoom.Users." + license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey);
            var sw = Stopwatch.StartNew();
            var cacheData = await _cache.GetAsync(cacheKey);
            sw.Stop();
            _logger.InfoFormat($"[GetActiveUsers:{license.ConsumerKey}] Get Time: {sw.Elapsed}");
            if (cacheData == null)
            {
                _logger.Info($"[GetActiveUsers:{license.ConsumerKey}] Empty cache data");
                sw = Stopwatch.StartNew();
                await StaticStorage.NamedLocker.WaitAsync(license.ConsumerKey);
                try
                {
                    cacheData = await _cache.GetAsync(cacheKey);
                    if (cacheData == null)
                    {
                        await _cacheUpdater.UpdateUsers(license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey), _zoomApi);
                        cacheData = await _cache.GetAsync(cacheKey);
                    }
                }
                finally
                {
                    StaticStorage.NamedLocker.Release(license.ConsumerKey);
                }
                sw.Stop();
                _logger.InfoFormat($"[GetActiveUsers:{license.ConsumerKey}] Update Time: {sw.Elapsed}");
            }

            var json = Encoding.UTF8.GetString(cacheData);
            List<User> result = _jsonDeserializer.JsonDeserialize<List<User>>(json);

            return result;
        }

        public UserInfoDto GetUser(string idOrEmail)
        {
            var user = _zoomApi.GetUser(idOrEmail);
            return user == null
                ? null
                : new UserInfoDto
                {
                    Id = user.Id,
                    Type = user.Type,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Verified = user.Verified == 1,
                    Timezone = user.Timezone
                };

        }

        public UserDto CreateUser(string email, string firstName, string lastName)
        {
            return CreateUser(new CreateUserDto()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
            });
        }

        public UserDto CreateUser(CreateUserDto dto)
        {
            var user = _zoomApi.CreateUser(new CreateUser
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password,
                Type = UserTypes.Basic,
            }, "create");

            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }


        public List<ZoomMeetingRegistrantDto> GetMeetingRegistrants(string meetingId, string occurrenceId = null, ZoomMeetingRegistrantStatus? status = null)
        {
            var registrants = new List<ZoomMeetingRegistrantDto>();
            var statuses = new List<ZoomMeetingRegistrantStatus>();
            if (status.HasValue)
                statuses.Add(status.Value);
            else
            {
                statuses.AddRange(new[] { ZoomMeetingRegistrantStatus.Approved, ZoomMeetingRegistrantStatus.Denied, ZoomMeetingRegistrantStatus.Pending });
            }
            foreach (var st in statuses)
            {
                var pageNumber = 1;
                var pageSize = 300;
                var totalRecords = 0;

                do
                {
                    var page = _zoomApi.GetMeetingRegistrants(meetingId, occurrenceId, st.ToString().ToLower(), pageSize, pageNumber);
                    registrants.AddRange(page.Registrants.Select(x => ConvertFromApiObjectToDto(x)));
                    totalRecords = page.TotalRecords;
                    pageNumber++;
                } while (pageSize * (pageNumber - 1) < totalRecords);

            }

            return registrants;
        }

        public async Task<bool> RegisterUsersToMeetingAndApprove(string meetingId, IEnumerable<RegistrantDto> registrants, bool checkRegistrants)
        {
            var registrantsToApprove = new List<ZoomMeetingRegistrantDto>();
            var zoomUserEmails = (await GetActiveUsers()).Select(x =>x.Email);
            var regs = GetMeetingRegistrants(meetingId);

            foreach (var registrant in registrants)
            {
                try
                {
                    if (zoomUserEmails.Any(x =>
                        x.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        bool addRegistrant = !checkRegistrants;
                        if (checkRegistrants)
                        {
                            var reg = regs.FirstOrDefault(x =>
                                x.Email.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase));
                            if (reg == null)
                            {
                                addRegistrant = true;
                            }
                            else
                            {
                                if (reg.Status == ZoomMeetingRegistrantStatus.Approval ||
                                    reg.Status == ZoomMeetingRegistrantStatus.All
                                    || reg.Status == ZoomMeetingRegistrantStatus.Pending ||
                                    reg.Status == ZoomMeetingRegistrantStatus.Denied)
                                {
                                    registrantsToApprove.Add(reg);
                                }
                            }
                        }

                        if (addRegistrant)
                        {
                            var newZoomAddRegistrantRequest = new ZoomAddRegistrantRequest(registrant.Email,
                                registrant.FirstName, registrant.LastName);
                            var addResult = _zoomApi.AddRegistrant(meetingId, newZoomAddRegistrantRequest);
                            if (!addResult.IsSuccess)
                            {
                                continue;
                            }

                            registrantsToApprove.Add(new ZoomMeetingRegistrantDto
                            {
                                Id = addResult.Data.RegistrantId,
                                Email = registrant.Email
                            });
                        }
                    }
                    else
                    {
                        CreateUser(registrant.Email, registrant.FirstName, registrant.LastName);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"[RegisterAndApprove] meetingId={meetingId}, email={registrant.Email}", e);
                }
            }

            if (registrantsToApprove.Any())
            {
                UpdateRegistrantStatus(meetingId, registrantsToApprove.Select(r => r.Email), nameof(RegistrantUpdateStatusAction.Approve));
            }

            return true;
        }

        public void CleanApprovedRegistrant(string meetingId, IEnumerable<RegistrantDto> updatedRegistrants, List<ZoomMeetingRegistrantDto> regs)
        {
            var approvedRegistrants = regs.Where(r => r.Status == ZoomMeetingRegistrantStatus.Approved);

            var deletedRegistrants = approvedRegistrants.Where(r =>
                !updatedRegistrants.Any(newReg => newReg.Email.Equals(r.Email, StringComparison.CurrentCultureIgnoreCase)));

            if (deletedRegistrants.Any())
            {
                UpdateRegistrantStatus(meetingId, deletedRegistrants.Select(dr => dr.Email), nameof(RegistrantUpdateStatusAction.Deny));
            }
        }

        public void UpdateRegistrantStatus(string meetingId, string email, string status)
        {
            var registrants = new List<ZoomRegistrantForStatusRequest>();
            registrants.Add(new ZoomRegistrantForStatusRequest
            {
                Email = email
            });

            _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
                                                            {
                                                                Action = status,
                                                                Registrants = registrants
                                                            }, null);
        }

        public void UpdateRegistrantStatus(string meetingId, IEnumerable<string> emails, string status)
        {
            status = status.ToLower();
            var registrants = emails.Select(email => new ZoomRegistrantForStatusRequest
                {
                    Email = email
                })
                .ToList();

            _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
            {
                Action = status,
                Registrants = registrants
            }, null);
        }


        private ZoomMeetingRegistrantDto ConvertFromApiObjectToDto(ZoomMeetingRegistrant apiObject)
        {
            return new ZoomMeetingRegistrantDto
            {
                Id = apiObject.Id,
                Email = apiObject.Email,
                FirstName = apiObject.FirstName,
                LastName = apiObject.LastName,
                JoinUrl = apiObject.JoinUrl,
                Status = (ZoomMeetingRegistrantStatus) Enum.Parse(typeof(ZoomMeetingRegistrantStatus), apiObject.Status,
                    true),
                CreateTime = apiObject.CreateTime.DateTime,
            };
        }

    }

}