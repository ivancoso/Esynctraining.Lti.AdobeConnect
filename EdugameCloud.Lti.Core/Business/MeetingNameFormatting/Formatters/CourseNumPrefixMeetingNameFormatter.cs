using System;
using EdugameCloud.Core;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // CourseNum: MeetingTitle
    internal sealed class CourseNumPrefixMeetingNameFormatter : IMeetingNameFormatter
    {
        private readonly IJsonSerializer _nameInfoSerializer;


        public string FormatName { get { return "[Course Label]: [Meeting Title]"; } }


        public CourseNumPrefixMeetingNameFormatter(IJsonSerializer nameInfoSerializer)
        {
            if (nameInfoSerializer == null)
                throw new ArgumentNullException(nameof(nameInfoSerializer));
            _nameInfoSerializer = nameInfoSerializer;
        }


        public string BuildName(MeetingDTOLtiBase<MeetingSessionDTO> meeting, LtiParamDTO param, string courseId)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));

            if (meeting.GetMeetingType() == LmsMeetingType.OfficeHours)
                return meeting.Name.TruncateIfMoreThen(60);

            if (param.context_label.Length > 53)
                throw new WarningMessageException("Can't generate Adobe Connect meeting name. Course Label is too long.");

            return string.Format("{0}: {1}", param.context_label, meeting.Name).TruncateIfMoreThen(60);
        }

        public string UpdateName(LmsCourseMeeting meeting, string lmsMeetingTitle)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));
            if (string.IsNullOrWhiteSpace(lmsMeetingTitle))
                throw new ArgumentException("Meeting Title is required", nameof(lmsMeetingTitle));

            MeetingNameInfo nameInfo = _nameInfoSerializer.JsonDeserialize<MeetingNameInfo>(meeting.MeetingNameJson);
            nameInfo.meetingName = lmsMeetingTitle;
            meeting.MeetingNameJson = _nameInfoSerializer.JsonSerialize(nameInfo);

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
                return lmsMeetingTitle.TruncateIfMoreThen(60);

            return string.Format("{0}: {1}", (string)nameInfo.courseNum, lmsMeetingTitle).TruncateIfMoreThen(60);
        }

    }

}
