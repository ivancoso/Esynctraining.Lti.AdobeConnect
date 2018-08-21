using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.Extensions.Caching.Distributed;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomUserService
    {
        private readonly ZoomApiWrapper _zoomApi;
        private readonly IDistributedCache _cache;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly dynamic _settings;

        public ZoomUserService(ZoomApiWrapper zoomApi, IDistributedCache cache, ILmsLicenseAccessor licenseAccessor, ApplicationSettingsProvider settings)
        {
            _zoomApi = zoomApi ?? throw new ArgumentNullException(nameof(zoomApi));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _settings = settings;
        }

        public async Task<IEnumerable<string>> GetUsersEmails(UserStatuses status = UserStatuses.Active)
        {
            var license = await _licenseAccessor.GetLicense();
            var cacheKey = "Zoom.Users." + license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey);
            IFormatter formatter = new BinaryFormatter();
            var userEmailsBytes = await _cache.GetAsync(cacheKey);
            IEnumerable<string> result = null;
            if (userEmailsBytes == null)
            {
                result = GetUsersFromApi(status).Select(x => x.Email).ToArray();
                var cacheEntryOption = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(double.Parse(_settings.LicenseUsersCacheDuration))
                };
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, result);
                    userEmailsBytes = ms.ToArray();
                }

                await _cache.SetAsync(cacheKey, userEmailsBytes, cacheEntryOption);
            }
            else
            {
                using (var memStream = new MemoryStream())
                {
                    memStream.Write(userEmailsBytes, 0, userEmailsBytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    result = (string[])formatter.Deserialize(memStream);
                }
            }

            return result;
        }

        public List<User> GetUsersFromApi(UserStatuses status)
        {
            var users = new List<User>();
            var pageNumber = 1;
            var pageSize = 300;
            var totalRecords = 0;
            do
            {
                var page = _zoomApi.GetUsers(status, pageSize: pageSize, pageNumber: pageNumber);
                users.AddRange(page.Users);
                totalRecords = page.TotalRecords;
                pageNumber++;

            } while (pageSize * (pageNumber - 1) < totalRecords);

            return users;
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
                var users = _zoomApi.GetMeetingRegistrants(meetingId, occurrenceId, st.ToString().ToLower());
                registrants.AddRange(users.Registrants.Select(x => ConvertFromApiObjectToDto(x)));
            }

            return registrants;
        }

        public async Task<bool> RegisterUsersToMeetingAndApprove(string meetingId, IEnumerable<RegistrantDto> registrants, bool checkRegistrants)
        {
            var registrantsToApprove = new List<ZoomMeetingRegistrantDto>();
            var zoomUserEmails = await GetUsersEmails();
            foreach (var registrant in registrants)
            {
                if (zoomUserEmails.Any(x => x.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    bool addRegistrant = !checkRegistrants;
                    if (checkRegistrants)
                    {
                        var regs = GetMeetingRegistrants(meetingId);
                        var reg = regs.FirstOrDefault(x =>
                            x.Email.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase));
                        if (reg == null)
                        {
                            addRegistrant = true;
                        }
                        else
                        {
                            if (reg.Status == ZoomMeetingRegistrantStatus.Pending)
                            {
                                registrantsToApprove.Add(reg);
                            }
                        }
                    }

                    if (addRegistrant)
                    {
                        var newZoomAddRegistrantRequest = new ZoomAddRegistrantRequest(registrant.Email, registrant.FirstName, registrant.LastName);
                        var addResult = _zoomApi.AddRegistrant(meetingId, newZoomAddRegistrantRequest);
                        if (!addResult.IsSuccess)
                        {
                            continue;
                        }

                        registrantsToApprove.Add(new ZoomMeetingRegistrantDto{Id = addResult.Data.RegistrantId, Email = registrant.Email });

                    }
                }
                else
                {
                    //create user
                    try
                    {
                        var userInfo = CreateUser(new CreateUserDto
                        {
                            Email = registrant.Email,
                            FirstName = registrant.FirstName,
                            LastName = registrant.LastName,
                        });
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }

            if (registrantsToApprove.Any())
            {
                _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
                {
                    Action = "approve",
                    Registrants = registrantsToApprove.Select(x => new ZoomRegistrantForStatusRequest
                    {
                        Email = x.Email,
                        Id = x.Id,
                    }).ToList()
                }, null);
            }

            return true;
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