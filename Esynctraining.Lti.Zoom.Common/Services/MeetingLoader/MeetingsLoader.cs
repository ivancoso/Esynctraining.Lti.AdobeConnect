﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;

namespace Esynctraining.Lti.Zoom.Common.Services.MeetingLoader
{
    public abstract class MeetingsLoader
    {
        protected readonly ZoomDbContext _dbContext;
        private readonly string _courseId;
        protected readonly ZoomApiWrapper _zoomApi;
        protected readonly string _currentUserId;
        protected readonly LmsLicenseDto _license;
        protected readonly UserInfoDto _user;
        private readonly ZoomUserService _zoomUserService;

        protected CourseMeetingType CourseMeetingType { get; set; }

        protected MeetingsLoader(ZoomDbContext dbContext, LmsLicenseDto license, string courseId, ZoomApiWrapper zoomApi, string currentUserId, UserInfoDto user, ZoomUserService zoomUserService)
        {
            _dbContext = dbContext;
            _license = license;
            _courseId = courseId;
            _zoomApi = zoomApi;
            _currentUserId = currentUserId;
            _user = user;
            _zoomUserService = zoomUserService;
        }

        public async Task<IEnumerable<MeetingViewModel>> Load()
        {
            var dbMeetings = await LoadFromDb();
            var mergedMeetings = await MergeWithZoomMeeting(dbMeetings);
            var processedMeetings = await ProcessMergedMeetings(mergedMeetings);
            return processedMeetings;
        }

        protected abstract Task<IEnumerable<MeetingViewModel>> ProcessMergedMeetings(List<MeetingViewModel> meetings);

        protected async Task<IEnumerable<LmsCourseMeeting>> LoadFromDb()
        {
            var dbMeetings = _dbContext.LmsCourseMeetings.Where(x =>
                x.LicenseKey == _license.ConsumerKey && _courseId == x.CourseId && x.Type == (int)CourseMeetingType);

            return dbMeetings;
        }

        protected async Task<List<MeetingViewModel>> MergeWithZoomMeeting(IEnumerable<LmsCourseMeeting> dbMeetings)
        {
            bool enableSubAccounts = _license.GetSetting<bool>(LmsLicenseSettingNames.EnableSubAccounts);

            List<MeetingViewModel> result = new List<MeetingViewModel>();
            var notHandledMeetings = dbMeetings.Select(x => x.ProviderMeetingId).ToList();
            foreach (var notHandledId in notHandledMeetings)
            {
                var meetingDetailResult = await _zoomApi.GetMeeting(notHandledId);

                if (!meetingDetailResult.IsSuccess && enableSubAccounts)
                {
                    var dbMeeting = dbMeetings.First(m => m.ProviderMeetingId == notHandledId);
                    meetingDetailResult = await _zoomApi.GetMeeting(dbMeeting.SubAccountId, notHandledId);
                }

                if (!meetingDetailResult.IsSuccess)
                {
                    var deleteErrorCodes = new List<ZoomApiErrorCodes>
                    {
                        ZoomApiErrorCodes.MeetingNotFound,
                        ZoomApiErrorCodes.UserNotFound,
                        ZoomApiErrorCodes.UserNotBelongToAccount
                    };

                    if (deleteErrorCodes.Any(x => (int)x == meetingDetailResult.Code))
                    {
                        await DeleteMeeting(dbMeetings.First(db => db.ProviderMeetingId == notHandledId));
                    }

                    continue;
                }

                if (meetingDetailResult.Data != null)
                {
                    var dbMeeting = dbMeetings.First(db => db.ProviderMeetingId == meetingDetailResult.Data.Id);
                    var vm = ConvertToViewModel(meetingDetailResult.Data, dbMeeting, _currentUserId);
                    result.Add(vm);
                }
            }

            return result;
        }

        #region Private methods

        private async Task DeleteMeeting(LmsCourseMeeting meeting)
        {
            _dbContext.Remove(meeting);
            await _dbContext.SaveChangesAsync();
        }

        private MeetingViewModel ConvertToViewModel(Meeting meeting, LmsCourseMeeting dbMeeting, string userId)
        {

            var vm = new MeetingViewModel
            {
                ConferenceId = meeting.Id,
                CanJoin = userId != null,
                CanEdit = meeting.HostId == userId,
                HostId = meeting.HostId,
                Duration = meeting.Duration,
                Timezone = meeting.Timezone,
                Topic = meeting.Topic,
                StartTime = GetUtcTime(meeting),
                HasSessions = meeting.Type == MeetingTypes.RecurringWithTime,
            };

            if (dbMeeting != null)
            {
                vm.Id = dbMeeting.Id;
                vm.Type = dbMeeting.Type;
                vm.LtiSessions = dbMeeting.MeetingSessions.Select(MeetingSessionConverter.ConvertFromEntity);
            }

            return vm;
        }

        protected long GetUtcTime(Meeting meeting)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            return meeting.StartTime.ToUnixTimeMilliseconds();

            //todo: remove method. It looks like time is coming in UTC from API
        }

        #endregion Private methods

    }
}
