using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
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
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(double.Parse(_settings.LicenseUsersCacheDuration))
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
            var zoomUserEmails = await GetUsersEmails();
            var regs = GetMeetingRegistrants(meetingId);

            CleanApprovedRegistrant(meetingId, registrants, regs);

            foreach (var registrant in registrants)
            {
                if (zoomUserEmails.Any(x => x.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    bool addRegistrant = !checkRegistrants;
                    if (checkRegistrants)
                    {
                        var reg = regs.FirstOrDefault(x => x.Email.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase));
                        if (reg == null)
                        {
                            addRegistrant = true;
                        }
                        else
                        {
                            if (reg.Status == ZoomMeetingRegistrantStatus.Approval || reg.Status == ZoomMeetingRegistrantStatus.Approval || reg.Status == ZoomMeetingRegistrantStatus.Pending || reg.Status == ZoomMeetingRegistrantStatus.Denied)
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
                    CreateUser(registrant.Email, registrant.FirstName, registrant.LastName);
                }
            }

            if (registrantsToApprove.Any())
            {
                UpdateRegistrantStatus(meetingId, registrantsToApprove.Select(r => r.Email), nameof(RegistrantUpdateStatusAction.Approve));
            }

            return true;
        }

        private void CleanApprovedRegistrant(string meetingId, IEnumerable<RegistrantDto> updatedRegistrants, List<ZoomMeetingRegistrantDto> regs)
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