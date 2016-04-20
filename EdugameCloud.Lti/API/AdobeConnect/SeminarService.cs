using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class SeminarService : Esynctraining.AdobeConnect.SeminarService, ISeminarService
    {
        private UsersSetup UsersSetup
        {
            get { return IoC.Resolve<UsersSetup>(); }
        }


        public SeminarService(ILogger logger)
            : base(logger)
        {
        }

        public IEnumerable<SeminarLicenseDto> GetLicensesWithContent(IAdobeConnectProxy acProxy,
            IEnumerable<LmsCourseMeeting> seminarRecords,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany,
            TimeZoneInfo timeZone)
        {
            var licenseDtos = new List<SeminarLicenseDto>();

            var licenses = GetSharedOrUserSeminarLicenses(acProxy).Where(x => !x.IsExpired);
            foreach (var license in licenses)
            {
                var seminars = GetSeminars(license.ScoId, acProxy);
                var rooms = new List<SeminarDto>();
                foreach (ScoContent seminar in seminars)
                {
                    LmsCourseMeeting meetingRecord = seminarRecords.FirstOrDefault(x => x.ScoId == seminar.ScoId);
                    if (meetingRecord == null)
                        continue;
                    //.Where(x => seminarRecords.Any(sr => sr.ScoId == x.ScoId))

                    var sessions = GetSeminarSessions(seminar.ScoId, acProxy);

                    var room = GetDtoByScoInfo(acProxy, lmsUser, param, lmsCompany, seminar, timeZone);
                    room.id = meetingRecord.Id; // TRICK: within LTI we use RECORD ID - not original SCO-ID!!

                    room.Sessions = sessions.Select(x => new SeminarSessionDto
                    {
                        // TRICK: within LTI we use RECORD ID - not original SCO-ID!!
                        id = x.ScoId,

                        name = x.Name,
                        //start_date = x.BeginDate.ToString("yyyy-MM-dd"),
                        //start_time = x.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
                        start_timestamp = (long)x.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timeZone, x.BeginDate),
                        duration = (x.EndDate - x.BeginDate).ToString(@"h\:mm"),
                        summary = x.Description,
                        ac_room_url = x.UrlPath.Trim('/'),
                        is_editable = true,

                        // TRICK: within LTI we use RECORD ID - not original SCO-ID!!
                        seminarRoomId = meetingRecord.Id.ToString(),
                    }).ToArray();
                    rooms.Add(room);
                }

                var dto = new SeminarLicenseDto
                {
                    Id = license.ScoId,
                    Name = license.Name,
                    Rooms = rooms.ToArray(),
                };
                licenseDtos.Add(dto);
            }

            return licenseDtos;
        }

        public OperationResultWithData<SeminarSessionDto> SaveSeminarSession(SeminarSessionDto seminarSessionDto, 
            IAdobeConnectProxy provider,
            TimeZoneInfo timeZone)
        {
            FixDateTimeFields(seminarSessionDto);

            var updateItem = new Esynctraining.AdobeConnect.SeminarSessionDto
            {
                Name = seminarSessionDto.name,
                SeminarScoId = seminarSessionDto.seminarRoomId,
                ExpectedLoad = seminarSessionDto.ExpectedLoad, 
            };

            if (string.IsNullOrEmpty(seminarSessionDto.start_date) || string.IsNullOrEmpty(seminarSessionDto.start_time))
            {
                updateItem.DateBegin = DateTime.Now;
                updateItem.DateEnd = DateTime.Now.AddDays(1);
            }

            bool isNewSeminar = string.IsNullOrEmpty(seminarSessionDto.id);
            if (!isNewSeminar)
            {
                updateItem.SeminarSessionScoId = seminarSessionDto.id;
            }
            
            DateTime dateBegin;
            if (DateTime.TryParse(seminarSessionDto.start_date + " " + seminarSessionDto.start_time, out dateBegin))
            {
                updateItem.DateBegin = dateBegin;
                TimeSpan duration;
                if (TimeSpan.TryParse(seminarSessionDto.duration, out duration))
                {
                    updateItem.DateEnd =
                        dateBegin.AddMinutes((int)duration.TotalMinutes);
                }
            }

            var scoInfo = SaveSession(updateItem, provider);
            var newSessionDto = new SeminarSessionDto
            {
                id = scoInfo.ScoId,
                name = scoInfo.Name,
                start_date = scoInfo.BeginDate.ToString("yyyy-MM-dd"),
                start_time = scoInfo.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
                start_timestamp = (long)scoInfo.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timeZone, scoInfo.BeginDate),
                duration = (scoInfo.EndDate - scoInfo.BeginDate).ToString(@"h\:mm"),
                summary = scoInfo.Description,
                ac_room_url = scoInfo.UrlPath.Trim('/'),
                is_editable = scoInfo.BeginDate.ToUniversalTime() > DateTime.UtcNow,
                seminarRoomId = seminarSessionDto.seminarRoomId,
            };
            return OperationResultWithData<SeminarSessionDto>.Success(newSessionDto);
        }

        private SeminarDto GetDtoByScoInfo(
            IAdobeConnectProxy provider,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany,
            ScoContent seminar,
            TimeZoneInfo timeZone,
            StringBuilder trace = null)
        {
            var psw = Stopwatch.StartNew();

            bool meetingExistsInAC;
            IEnumerable<PermissionInfo> permission = provider.GetMeetingPermissions(seminar.ScoId,
                new List<string> { "public-access", lmsUser.PrincipalId },
                out meetingExistsInAC).Values;

            psw.Stop();
            if (trace != null)
                trace.AppendFormat("\t GetMeetings - AC GetMeetingPermissions time: {0}. MeetingId: {1}\r\n", psw.Elapsed.ToString(), seminar.ScoId);

            if (!meetingExistsInAC)
                return null;

            bool isEditable = this.UsersSetup.IsTeacher(param);
            var canJoin = this.CanJoin(lmsUser, permission) || GetGuestAuditRoleMappings(lmsCompany, param).Any();

            PermissionInfo permissionInfo = permission != null ? permission.FirstOrDefault(x => x.PrincipalId == "public-access" && x.PermissionId != PermissionId.none) : null;

            var sw = Stopwatch.StartNew();

            sw.Stop();
            if (trace != null)
                trace.AppendFormat("\t GetMeetings - DB GetByCompanyAndScoId time: {0}. MeetingId: {1}\r\n", sw.Elapsed.ToString(), seminar.ScoId);

            var scoInfo = provider.GetScoInfo(seminar.ScoId);

            var ret = new SeminarDto
            {
                id = long.Parse(seminar.ScoId),
                ac_room_url = seminar.UrlPath.Trim("/".ToCharArray()),
                name = seminar.Name,
                summary = seminar.Description,
                template = seminar.SourceScoId,
                // HACK: localization
                //start_date = seminar.BeginDate.ToString("yyyy-MM-dd"),
                //start_time = seminar.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
                start_timestamp = (long)seminar.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timeZone, seminar.BeginDate),
                duration = (seminar.EndDate - seminar.BeginDate).ToString(@"h\:mm"),
                access_level = permissionInfo != null ? permissionInfo.PermissionId.ToString() : "remove",
                allow_guests = permissionInfo == null || permissionInfo.PermissionId == PermissionId.remove,
                can_join = canJoin,
                is_editable = isEditable,
                type = (int)LmsMeetingType.Seminar,
                office_hours = null,
                reused = false,

                audioProfileId = scoInfo.ScoInfo.TelephonyProfile, // TODO: ???seminar.AudioProfileId
                SeminarLicenseId = seminar.FolderId.ToString(),
            };
            return ret;
        }


        private bool CanJoin(
            LmsUser lmsUser,
            IEnumerable<PermissionInfo> permission)
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
                .Intersect(new List<PermissionId> { PermissionId.host, PermissionId.mini_host, PermissionId.view })
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

        private static void FixDateTimeFields(SeminarSessionDto seminarSessionDto)
        {
            if (seminarSessionDto.start_time != null)
            {
                seminarSessionDto.start_time = seminarSessionDto.start_time.PadLeft(8, '0');
            }

            if (seminarSessionDto.start_date != null)
            {
                seminarSessionDto.start_date = seminarSessionDto.start_date.Substring(6, 4) + "-"
                                        + seminarSessionDto.start_date.Substring(0, 5);
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

        //public OperationResult<SeminarSessionDto> SaveSeminarSession(SeminarSessionDto seminarSessionDto, IAdobeConnectProxy adminProvider,
        //    IAdobeConnectProxy userProvider, string acUsername)
        //{
        //    FixDateTimeFields(seminarSessionDto);

        //    string seminarScoId = seminarSessionDto.id;
        //    var seminarInfo = userProvider.GetScoInfo(seminarScoId);

        //    bool isNewSeminar = String.IsNullOrEmpty(seminarScoId);

        //    var updateItem = new AdobeConnect.SeminarSessionDto
        //    {
        //        Name = seminarSessionDto.name,
        //        SeminarScoId = seminarSessionDto.seminarRoomId
        //    };
        //    if (seminarSessionDto.start_date == null || seminarSessionDto.start_time == null)
        //    {
        //        updateItem.DateBegin = DateTime.Now;
        //        updateItem.DateEnd = DateTime.Now.AddDays(1);
        //    }
        //    if (!isNewSeminar)
        //    {
        //        updateItem.SeminarSessionScoId = seminarSessionDto.id;
        //    }

        //    DateTime dateBegin;

        //    if (DateTime.TryParse(seminarSessionDto.start_date + " " + seminarSessionDto.start_time, out dateBegin))
        //    {
        //        updateItem.DateBegin = dateBegin;
        //        TimeSpan duration;
        //        if (TimeSpan.TryParse(seminarSessionDto.duration, out duration))
        //        {
        //            updateItem.DateEnd =
        //                dateBegin.AddMinutes((int)duration.TotalMinutes);
        //        }
        //    }

        //    var scoInfo = SaveSession(updateItem, adminProvider);

        //    return OperationResult<SeminarSessionDto>.Success(seminarSessionDto);
        //}

        //private static double ConvertToUnixTimestamp(DateTime value)
        //{
        //    var _origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    var _originLocalTime = _origin.ToLocalTime();
        //    if (value.Kind != DateTimeKind.Utc)
        //    {
        //        return (value - _originLocalTime).TotalSeconds * 1000;
        //    }

        //    return (value - _origin).TotalSeconds * 1000;
        //}

    }

}
