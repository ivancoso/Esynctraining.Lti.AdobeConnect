using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class SeminarService : Esynctraining.AdobeConnect.SeminarService, ISeminarService
    {
        // TODO: reuse from Esynctraining.AdobeConnect.SeminarService
        private readonly ILogger _logger;

        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();


        public SeminarService(ILogger logger)
            : base(logger)
        {
            _logger = logger;
        }


        public IEnumerable<SeminarLicenseDto> GetLicensesWithContent(IAdobeConnectProxy acProxy,
            IEnumerable<LmsCourseMeeting> seminarRecords,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany,
            TimeZoneInfo timeZone)
        {
            if (acProxy == null)
                throw new ArgumentNullException(nameof(acProxy));
            if (seminarRecords == null)
                throw new ArgumentNullException(nameof(seminarRecords));
            if (lmsUser == null)
                throw new ArgumentNullException(nameof(lmsUser));
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            var licenseDtos = new List<SeminarLicenseDto>();

            var sharedLicenses = GetSharedSeminarLicenses(acProxy).Where(x => !x.IsExpired);
            foreach (var license in sharedLicenses)
            {
                PopulateLicenseRooms(
                    license.ScoId, license.Name,
                    ref licenseDtos,
                    acProxy,
                    seminarRecords,
                    lmsUser,
                    param,
                    lmsCompany,
                    timeZone);
            }

            try
            {
                var userLicenses = GetUserSeminarLicenses(acProxy).Where(x => x.PrincipalId == lmsUser.PrincipalId);
                foreach (var license in userLicenses)
                {
                    PopulateLicenseRooms(
                        license.ScoId, license.Name,
                        ref licenseDtos,
                        acProxy,
                        seminarRecords,
                        lmsUser,
                        param,
                        lmsCompany,
                        timeZone);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("[GetUserSeminarLicenses] error", ex);
            }

            return licenseDtos;
        }

        public OperationResultWithData<SeminarSessionDto> SaveSeminarSession(SeminarSessionInputDto seminarSessionDto, 
            string seminarScoId,
            IAdobeConnectProxy provider,
            TimeZoneInfo timeZone)
        {
            FixDateTimeFields(seminarSessionDto);

            var updateItem = new Esynctraining.AdobeConnect.SeminarSessionDto
            {
                Name = seminarSessionDto.Name,
                Summary = seminarSessionDto.Summary,
                SeminarScoId = seminarScoId,
                ExpectedLoad = seminarSessionDto.ExpectedLoad, 
            };

            if (string.IsNullOrEmpty(seminarSessionDto.StartDate) || string.IsNullOrEmpty(seminarSessionDto.StartTime))
            {
                updateItem.DateBegin = DateTime.Now;
                updateItem.DateEnd = DateTime.Now.AddDays(1);
            }

            bool isNewSeminar = string.IsNullOrEmpty(seminarSessionDto.Id);
            if (!isNewSeminar)
            {
                updateItem.SeminarSessionScoId = seminarSessionDto.Id;
            }
            
            DateTime dateBegin;
            if (DateTime.TryParse(seminarSessionDto.StartDate + " " + seminarSessionDto.StartTime, out dateBegin))
            {
                updateItem.DateBegin = dateBegin;
                TimeSpan duration;
                if (TimeSpan.TryParse(seminarSessionDto.Duration, out duration))
                {
                    updateItem.DateEnd =
                        dateBegin.AddMinutes((int)duration.TotalMinutes);
                }
            }

            var scoInfo = SaveSession(updateItem, provider);
            var newSessionDto = new SeminarSessionDto
            {
                Id = scoInfo.ScoId,
                Name = scoInfo.Name,
                StartTimeStamp = (long)scoInfo.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timeZone, scoInfo.BeginDate),
                Duration = (scoInfo.EndDate - scoInfo.BeginDate).ToString(@"h\:mm"),
                Summary = scoInfo.Description,
                AcRoomUrl = scoInfo.UrlPath.Trim('/'),
                IsEditable = scoInfo.BeginDate.ToUniversalTime() > DateTime.UtcNow,
                SeminarRoomId = seminarSessionDto.SeminarRoomId,
            };
            return newSessionDto.ToSuccessResult();
        }

        private void PopulateLicenseRooms(
            string licenseScoId, string licenseName,
            ref List<SeminarLicenseDto> licenseDtos,
            IAdobeConnectProxy acProxy,
            IEnumerable<LmsCourseMeeting> seminarRecords,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany,
            TimeZoneInfo timeZone)
        {
            var seminars = GetSeminars(licenseScoId, acProxy);
            var rooms = new List<SeminarDto>();
            foreach (ScoContent seminar in seminars)
            {
                LmsCourseMeeting meetingRecord = seminarRecords.FirstOrDefault(x => x.ScoId == seminar.ScoId);
                if (meetingRecord == null)
                    continue;

                var sessions = GetSeminarSessions(seminar.ScoId, acProxy);

                var room = GetDtoByScoInfo(acProxy, lmsUser, param, lmsCompany, seminar, meetingRecord, timeZone);
                room.Id = meetingRecord.Id; // TRICK: within LTI we use RECORD ID - not original SCO-ID!!

                room.Sessions = sessions.Select(x => new SeminarSessionDto
                {
                    Id = x.ScoId,

                    Name = x.Name,
                    StartTimeStamp = (long)x.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timeZone, x.BeginDate),
                    Duration = (x.EndDate - x.BeginDate).ToString(@"h\:mm"),
                    Summary = x.Description,
                    AcRoomUrl = x.UrlPath.Trim('/'),
                    IsEditable = true,

                    // TRICK: within LTI we use RECORD ID - not original SCO-ID!!
                    SeminarRoomId = meetingRecord.Id.ToString(),
                }).ToArray();
                rooms.Add(room);
            }

            var dto = new SeminarLicenseDto
            {
                Id = licenseScoId,
                Name = licenseName,
                Rooms = rooms.ToArray(),
            };
            licenseDtos.Add(dto);
        }

        private SeminarDto GetDtoByScoInfo(
            IAdobeConnectProxy provider,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany,
            ScoContent seminar,
            LmsCourseMeeting seminarMeeting,
            TimeZoneInfo timeZone,
            StringBuilder trace = null)
        {
            var psw = Stopwatch.StartNew();

            bool meetingExistsInAC;
            IEnumerable<MeetingPermissionInfo> permission = provider.GetMeetingPermissions(seminar.ScoId,
                new List<string> { "public-access", lmsUser.PrincipalId },
                out meetingExistsInAC).Values;

            psw.Stop();
            if (trace != null)
                trace.AppendFormat("\t GetMeetings - AC GetMeetingPermissions time: {0}. MeetingId: {1}\r\n", psw.Elapsed.ToString(), seminar.ScoId);

            if (!meetingExistsInAC)
                return null;

            bool isEditable = this.UsersSetup.IsTeacher(param);
            var canJoin = this.CanJoin(lmsUser, permission) || GetGuestAuditRoleMappings(lmsCompany, param).Any()
                || (lmsCompany.UseSynchronizedUsers && seminarMeeting.EnableDynamicProvisioning);

            MeetingPermissionInfo permissionInfo = permission != null 
                ? permission.FirstOrDefault(x => x.PrincipalId == "public-access" && x.PermissionId != MeetingPermissionId.none) 
                : null;

            var sw = Stopwatch.StartNew();

            sw.Stop();
            if (trace != null)
                trace.AppendFormat("\t GetMeetings - DB GetByCompanyAndScoId time: {0}. MeetingId: {1}\r\n", sw.Elapsed.ToString(), seminar.ScoId);

            var scoInfo = provider.GetScoInfo(seminar.ScoId);

            var ret = new SeminarDto
            {
                Id = long.Parse(seminar.ScoId),
                AcRoomUrl = seminar.UrlPath.Trim("/".ToCharArray()),
                Name = seminar.Name,
                Summary = seminar.Description,
                Template = seminar.SourceScoId,
                StartTimeStamp = (long)seminar.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timeZone, seminar.BeginDate),
                Duration = (seminar.EndDate - seminar.BeginDate).ToString(@"h\:mm"),
                AccessLevel = permissionInfo != null ? permissionInfo.PermissionId.ToString() : "remove",
                CanJoin = canJoin,
                IsEditable = isEditable,
                Type = (int)LmsMeetingType.Seminar,
                OfficeHours = null,
                Reused = false,

                AudioProfileId = scoInfo.ScoInfo.TelephonyProfile, // TODO: ???seminar.AudioProfileId
                SeminarLicenseId = seminar.FolderId.ToString(),
            };
            return ret;
        }


        private bool CanJoin(
            LmsUser lmsUser,
            IEnumerable<MeetingPermissionInfo> permission)
        {
            // this method is called after the user has opened the application through LtiController, so there should already be Principal found and saved for the user.
            if (string.IsNullOrWhiteSpace(lmsUser.PrincipalId))
            {
                throw new InvalidOperationException(string.Format("lmsUser.PrincipalId is empty. LmsUserID: {0}", lmsUser.Id));
            }

            return (permission != null)
                && permission
                .Where(x => x.PrincipalId == lmsUser.PrincipalId)
                .Select(x => x.PermissionId)
                .Intersect(new List<MeetingPermissionId> { MeetingPermissionId.host, MeetingPermissionId.mini_host, MeetingPermissionId.view })
                .Any();
        }

        private IEnumerable<LmsCompanyRoleMapping> GetGuestAuditRoleMappings(LmsCompany lmsCompany, LtiParamDTO param)
        {
            if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableAuditGuestEntry))
                return Enumerable.Empty<LmsCompanyRoleMapping>();
            var customRoles = lmsCompany.RoleMappings.Where(x => !x.IsDefaultLmsRole && new[] { AcRole.Host.Id, AcRole.Presenter.Id }.Contains(x.AcRole));
            var currentUserLtiRoles = new List<string>();
            if (!string.IsNullOrEmpty(param.roles))
            {
                currentUserLtiRoles.AddRange(param.roles.Split(',', ';').Select(x => x.Trim()));
            }

            return customRoles.Where(x => currentUserLtiRoles.Any(lr => lr.Equals(x.LmsRoleName)));
        }

        private static void FixDateTimeFields(SeminarSessionInputDto seminarSessionDto)
        {
            if (seminarSessionDto.StartTime != null)
            {
                seminarSessionDto.StartTime = seminarSessionDto.StartTime.PadLeft(8, '0');
            }

            if (seminarSessionDto.StartDate != null)
            {
                seminarSessionDto.StartDate = seminarSessionDto.StartDate.Substring(6, 4) + "-"
                                        + seminarSessionDto.StartDate.Substring(0, 5);
            }
        }
        
        private double GetTimezoneShift(TimeZoneInfo timezone, DateTime value)
        {
            if (timezone != null)
            {
                var offset = timezone.GetUtcOffset(value).TotalMilliseconds;
                return offset;
            }

            return 0;
        }
        
    }

}
