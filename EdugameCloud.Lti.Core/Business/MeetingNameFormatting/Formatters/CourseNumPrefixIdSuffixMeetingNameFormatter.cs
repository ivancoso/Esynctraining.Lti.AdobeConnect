using System;
using EdugameCloud.Core;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // CourseNum: MeetingTitle - ID
    internal sealed class CourseNumPrefixIdSuffixMeetingNameFormatter : IMeetingNameFormatter
    {
        private readonly IJsonSerializer _nameInfoSerializer;


        public string FormatName { get { return "[Course Label]: [Meeting Title] - [ID]"; } }


        public CourseNumPrefixIdSuffixMeetingNameFormatter(IJsonSerializer nameInfoSerializer)
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

            int extraDataLength = param.context_label.Length + ": ".Length + " - ".Length + courseId.Length;
            if (extraDataLength > 52)
                throw new WarningMessageException("Can't generate Adobe Connect meeting name. Course Label is too long.");

            return string.Format("{0}: {1} - {2}", param.context_label, meeting.Name.TruncateIfMoreThen(60 - extraDataLength), courseId);
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

            int extraDataLength = ((string)nameInfo.courseNum).Length + ": ".Length + " - ".Length + ((string)nameInfo.courseId).Length;
            if (extraDataLength > 52)
                throw new WarningMessageException("Can't generate Adobe Connect meeting name. Course Label is too long.");

            return string.Format("{0}: {1} - {2}", (string)nameInfo.courseNum, lmsMeetingTitle.TruncateIfMoreThen(60 - extraDataLength), (string)nameInfo.courseId);
        }

    }

}
