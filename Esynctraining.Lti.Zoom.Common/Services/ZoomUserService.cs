using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class ZoomUserService
    {
        private readonly ZoomApiWrapper _zoomApi;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ILogger _logger;

        public ZoomUserService(ZoomApiWrapper zoomApi, ILmsLicenseAccessor licenseAccessor, ILogger logger)
        {
            _zoomApi = zoomApi ?? throw new ArgumentNullException(nameof(zoomApi));
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetActiveUsers(LmsLicenseDto licenseDto = null)
        {
            var license = licenseDto ?? await _licenseAccessor.GetLicense();
            var sw = Stopwatch.StartNew();
            var users = new List<User>();
            var pageNumber = 1;
            var pageSize = 300;
            var totalRecords = 0;
            do
            {
                var page = await _zoomApi.GetUsers(UserStatus.Active, pageSize: pageSize, pageNumber: pageNumber);
                users.AddRange(page.Users);
                totalRecords = page.TotalRecords;
                pageNumber++;

            } while (pageSize * (pageNumber - 1) < totalRecords);
            sw.Stop();
            _logger.InfoFormat($"[GetActiveUsers:{license.ConsumerKey}] Get Time: {sw.Elapsed}");
            
            return users;
        }

        public async Task<UserInfoDto> GetUser(string idOrEmail)
        {
            var user = await _zoomApi.GetUser(idOrEmail);
            return BuildUserInfoDto(user);
        }

        public async Task<UserInfoDto> GetUser(string accountId, string idOrEmail)
        {
            var user = await _zoomApi.GetUser(accountId, idOrEmail);
            return BuildUserInfoDto(user);
        }

        public async Task<IEnumerable<Account>> GetSubAccounts()
        {
            var subAccounts = new List<Account>();
            var pageNumber = 1;
            var pageSize = 300;
            var totalRecords = 0;
            do
            {
                var page = await _zoomApi.GetAccounts(pageSize: pageSize, pageNumber: pageNumber);
                subAccounts.AddRange(page.Accounts);
                totalRecords = page.TotalRecords;
                pageNumber++;

            } while (pageSize * (pageNumber - 1) < totalRecords);

            return subAccounts;
        }

        public async Task<UserInfoDto> GetUser(string idOrEmail, bool enableSubAccounts)
        {
            UserInfoDto user = await GetUser(idOrEmail);
            if (user != null || !enableSubAccounts)
                return user;

            var subAccounts = await GetSubAccounts();

            if (!subAccounts.Any())
                return null;

            foreach(var subAccount in subAccounts)
            {
                user = await GetUser(subAccount.Id, idOrEmail);
                if (user != null)
                {
                    user.SubAccountId = subAccount.Id;
                    break;
                }

                    
            }

            return user;
        }

        private UserInfoDto BuildUserInfoDto(UserInfo user)
        {
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
                    Timezone = user.Timezone,
                    Status = (ZoomUserStatus)((int)user.Status)
                };
        }

        public async Task<UserDto> CreateUser(string email, string firstName, string lastName)
        {
            return await CreateUser(new CreateUserDto()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
            });
        }

        public async Task<UserDto> CreateUser(CreateUserDto dto)
        {
            var user = await _zoomApi.CreateUser(new CreateUser
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

        public async Task<UserDto> CreateUser(string accountId, CreateUserDto dto)
        {
            var user = await _zoomApi.CreateUser(accountId, new CreateUser
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


        public async Task<List<ZoomMeetingRegistrantDto>> GetMeetingRegistrants(LmsCourseMeeting meeting, string occurrenceId = null, ZoomMeetingRegistrantStatus? status = null)
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
                    var page = string.IsNullOrEmpty(meeting.SubAccountId) 
                            ? await _zoomApi.GetMeetingRegistrants(meeting.ProviderMeetingId, occurrenceId, st.ToString().ToLower(), pageSize, pageNumber)
                            : await _zoomApi.GetSubAccountsMeetingRegistrants(meeting.SubAccountId, meeting.ProviderMeetingId, occurrenceId, st.ToString().ToLower(), pageSize, pageNumber);

                    registrants.AddRange(page.Registrants.Select(ConvertFromApiObjectToDto));
                    totalRecords = page.TotalRecords;
                    pageNumber++;
                } while (pageSize * (pageNumber - 1) < totalRecords);

            }

            return registrants;
        }

        public async Task<bool> RegisterUsersToMeetingAndApprove(LmsCourseMeeting meeting, IEnumerable<RegistrantDto> registrants, bool checkRegistrants)
        {
            var registrantsToApprove = new List<ZoomMeetingRegistrantDto>();
            var zoomUserEmails = (await GetActiveUsers()).Where(x => !string.IsNullOrEmpty(x.Email)).Select(x =>x.Email);
            var regs = await GetMeetingRegistrants(meeting);

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
                            var newZoomAddRegistrantRequest = new ZoomAddRegistrantRequest(registrant.Email, registrant.FirstName, registrant.LastName);
                            
                            var addResult = string.IsNullOrEmpty(meeting.SubAccountId) 
                                            ? await _zoomApi.AddRegistrant(meeting.ProviderMeetingId, newZoomAddRegistrantRequest)
                                            : await _zoomApi.AddRegistrant(meeting.SubAccountId, meeting.ProviderMeetingId, newZoomAddRegistrantRequest);

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
                        await CreateUser(registrant.Email, registrant.FirstName, registrant.LastName);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"[RegisterAndApprove] meetingId={meeting.ProviderMeetingId}, email={registrant.Email}", e);
                }
            }

            if (registrantsToApprove.Any())
            {
                await UpdateRegistrantStatus(meeting.SubAccountId ,meeting.ProviderMeetingId, registrantsToApprove.Select(r => r.Email), nameof(RegistrantUpdateStatusAction.Approve));
            }

            return true;
        }

        public async Task CleanApprovedRegistrant(string meetingId, IEnumerable<RegistrantDto> updatedRegistrants, List<ZoomMeetingRegistrantDto> regs)
        {
            var approvedRegistrants = regs.Where(r => r.Status == ZoomMeetingRegistrantStatus.Approved);

            var deletedRegistrants = approvedRegistrants.Where(r =>
                !updatedRegistrants.Any(newReg => newReg.Email.Equals(r.Email, StringComparison.CurrentCultureIgnoreCase)));

            if (deletedRegistrants.Any())
            {
                await UpdateRegistrantStatus(meetingId, deletedRegistrants.Select(dr => dr.Email), nameof(RegistrantUpdateStatusAction.Deny));
            }
        }

        public async Task UpdateRegistrantStatus(string meetingId, string email, string status)
        {
            var registrants = new List<ZoomRegistrantForStatusRequest>
            {
                new ZoomRegistrantForStatusRequest {Email = email}
            };

            await _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
                                                            {
                                                                Action = status,
                                                                Registrants = registrants
                                                            }, null);
        }

        public async Task UpdateRegistrantStatus(string subAccountId, string meetingId, IEnumerable<string> emails, string status)
        {
            status = status.ToLower();
            var registrants = emails.Select(email => new ZoomRegistrantForStatusRequest
            {
                Email = email
            })
                .ToList();

            if(string.IsNullOrEmpty(subAccountId))
            {
                await _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
                {
                    Action = status,
                    Registrants = registrants
                }, null);
            }
            else
            {
                await _zoomApi.UpdateRegistrantsStatus(subAccountId, meetingId, new ZoomUpdateRegistrantStatusRequest
                {
                    Action = status,
                    Registrants = registrants
                }, null);
            }
        }

        public async Task UpdateRegistrantStatus(string meetingId, IEnumerable<string> emails, string status)
        {
            status = status.ToLower();
            var registrants = emails.Select(email => new ZoomRegistrantForStatusRequest
                {
                    Email = email
                })
                .ToList();

            await _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
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