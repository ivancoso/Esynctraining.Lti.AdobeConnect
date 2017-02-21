using System;
using System.Collections.Generic;
using System.Globalization;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed partial class MeetingSetup
    {
        private static readonly Dictionary<LmsMeetingType, string> meetingNames = new Dictionary<LmsMeetingType, string>
        {
            { LmsMeetingType.Meeting, "Adobe Connect" },
            { LmsMeetingType.OfficeHours, "Office Hours" },
            { LmsMeetingType.StudyGroup, "Study Group" },
            { LmsMeetingType.Seminar, "Seminar Room" }
        };

        #region Properties

        private IBlackBoardApi BlackboardApi
        {
            get
            {
                return IoC.Resolve<IBlackBoardApi>();
            }
        }

        private ICanvasAPI CanvasApi
        {
            get
            {
                return IoC.Resolve<ICanvasAPI>();
            }
        }
        
        #endregion

        private void CreateAnnouncement(
            LmsMeetingType meetingType,
            LmsCompany lmsCompany, 
            LtiParamDTO param,
            MeetingDTOInput meetingDto)
        {
            if (!lmsCompany.ShowAnnouncements.GetValueOrDefault() || string.IsNullOrEmpty(param.context_title))
            {
                return;
            }

            var announcementTitle = string.Format(
                "A new {0} room was created for course {1}",
                meetingNames[meetingType],
                param.context_title);
            string announcementMessage = GetAnnouncementMessage(meetingType, meetingDto, param.referer);

            switch (lmsCompany.LmsProviderId)
            {
                case (int)LmsProviderEnum.Canvas:
                    var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                    var token = UsersSetup.IsTeacher(param) && !String.IsNullOrEmpty(lmsUser.Token)
                        ? lmsUser.Token
                        : lmsCompany.AdminUser.Return(a => a.Token, string.Empty);
                        
                    CanvasApi.CreateAnnouncement(
                        lmsCompany.LmsDomain,
                        token,
                        param.course_id,
                        announcementTitle,
                        announcementMessage);
                    break;
                case (int)LmsProviderEnum.Blackboard:
                    BlackboardApi.CreateAnnouncement(param.course_id, param.user_id, lmsCompany, announcementTitle, announcementMessage);
                    break;
                case (int)LmsProviderEnum.BrainHoney:
                    // string error;
//                    this.dlapApi.CreateAnnouncement(
//                        credentials,
//                        param.course_id,
//                        announcementTitle,
//                        announcementMessage, 
//                        out error);
                    break;
            }
        }

        private string GetAnnouncementMessage(LmsMeetingType meetingType, MeetingDTOInput meetingDto, string referrer)
        {
            //string startDate;
            //string startTime;
            //// TRICK: !!
            //var input = meetingDto as MeetingDTOInput;
            //if (input != null)
            //{
            //    startDate = input.StartDate;
            //    startTime = input.StartTime;
            //}
            //else
            //{
            //    DateTime begin = ((double)meetingDto.StartTimeStamp).ConvertFromUnixTimeStamp();
            //    //start_date = meeting.Sco.BeginDate.ToString("yyyy-MM-dd"),
            //    //start_time = meeting.Sco.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
            //}

            switch (meetingType)
            {
                case LmsMeetingType.Meeting:
                    string pattern = "Meeting \"{0}\" will start {1} at {2}. Its duration will be {3}. You can join it in your <a href='{4}'>Adobe Connect Conference section</a>.";
                    return string.Format(
                        pattern,
                        meetingDto.Name,
                        meetingDto.StartDate,
                        meetingDto.StartTime,
                        meetingDto.Duration,
                        referrer ?? string.Empty);
                case LmsMeetingType.OfficeHours:
                    string message = string.Format("You can join the meeting \"{0}\" in your <a href='{1}'>Adobe Connect Conference section</a>.", meetingDto.Name.Trim(), referrer ?? string.Empty);
                    if (!string.IsNullOrEmpty(meetingDto.OfficeHours))
                    {
                        message = string.Format("Meeting time: {0}. ", meetingDto.OfficeHours) + message;
                    }
                    return message;
                case LmsMeetingType.StudyGroup:
                    return string.Format("You can join the meeting \"{0}\" in your <a href='{1}'>Adobe Connect Conference section</a>.", meetingDto.Name.Trim(), referrer ?? string.Empty);

                case LmsMeetingType.Seminar:
                    return string.Format("You can join the seminar \"{0}\" in your <a href='{1}'>Adobe Connect Conference section</a>.", meetingDto.Name.Trim(), referrer ?? string.Empty);
            }

            return string.Empty;
        }

        private void CreateAnnouncement(
            LmsMeetingType meetingType,
            LmsCompany lmsCompany,
            LtiParamDTO param,
            MeetingDTO meetingDto,
            DateTime startDate)
        {
            if (!lmsCompany.ShowAnnouncements.GetValueOrDefault() || string.IsNullOrEmpty(param.context_title))
            {
                return;
            }

            var announcementTitle = string.Format(
                "A new {0} room was created for course {1}",
                meetingNames[meetingType],
                param.context_title);
            string announcementMessage = GetAnnouncementMessage(meetingType, meetingDto, param.referer, startDate);

            switch (lmsCompany.LmsProviderId)
            {
                case (int)LmsProviderEnum.Canvas:
                    var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                    var token = UsersSetup.IsTeacher(param) && !String.IsNullOrEmpty(lmsUser.Token)
                        ? lmsUser.Token
                        : lmsCompany.AdminUser.Return(a => a.Token, string.Empty);

                    CanvasApi.CreateAnnouncement(
                        lmsCompany.LmsDomain,
                        token,
                        param.course_id,
                        announcementTitle,
                        announcementMessage);
                    break;
                case (int)LmsProviderEnum.Blackboard:
                    BlackboardApi.CreateAnnouncement(param.course_id, param.user_id, lmsCompany, announcementTitle, announcementMessage);
                    break;
                case (int)LmsProviderEnum.BrainHoney:
                    // string error;
                    //                    this.dlapApi.CreateAnnouncement(
                    //                        credentials,
                    //                        param.course_id,
                    //                        announcementTitle,
                    //                        announcementMessage, 
                    //                        out error);
                    break;
            }
        }

        private string GetAnnouncementMessage(LmsMeetingType meetingType, MeetingDTO meetingDto, string referrer, DateTime startDate)
        {
            string date = startDate.ToString("yyyy-MM-dd");
            string time = startDate.ToString("h:mm tt", CultureInfo.InvariantCulture);

            switch (meetingType)
            {
                case LmsMeetingType.Meeting:
                    string pattern = "Meeting \"{0}\" will start {1} at {2}. Its duration will be {3}. You can join it in your <a href='{4}'>Adobe Connect Conference section</a>.";
                    return string.Format(
                        pattern,
                        meetingDto.Name,
                        date,
                        time,
                        meetingDto.Duration,
                        referrer ?? string.Empty);
                case LmsMeetingType.OfficeHours:
                    string message = string.Format("You can join the meeting \"{0}\" in your <a href='{1}'>Adobe Connect Conference section</a>.", meetingDto.Name.Trim(), referrer ?? string.Empty);
                    if (!string.IsNullOrEmpty(meetingDto.OfficeHours))
                    {
                        message = string.Format("Meeting time: {0}. ", meetingDto.OfficeHours) + message;
                    }
                    return message;
                case LmsMeetingType.StudyGroup:
                    return string.Format("You can join the meeting \"{0}\" in your <a href='{1}'>Adobe Connect Conference section</a>.", meetingDto.Name.Trim(), referrer ?? string.Empty);

                case LmsMeetingType.Seminar:
                    return string.Format("You can join the seminar \"{0}\" in your <a href='{1}'>Adobe Connect Conference section</a>.", meetingDto.Name.Trim(), referrer ?? string.Empty);
            }

            return string.Empty;
        }

    }

}