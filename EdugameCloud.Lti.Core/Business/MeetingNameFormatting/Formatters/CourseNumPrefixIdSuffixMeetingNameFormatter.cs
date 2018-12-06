using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // CourseNum: MeetingTitle - ID
    internal sealed class CourseNumPrefixIdSuffixMeetingNameFormatter : IMeetingNameFormatter
    {
        private readonly IJsonSerializer _nameInfoSerializer;
        private readonly IJsonDeserializer _nameInfoDeserializer;


        public string FormatName => "[Course Label]: [Meeting Title] - [ID]";


        public CourseNumPrefixIdSuffixMeetingNameFormatter(IJsonSerializer nameInfoSerializer, IJsonDeserializer nameInfoDeserializer)
        {
            _nameInfoSerializer = nameInfoSerializer ?? throw new ArgumentNullException(nameof(nameInfoSerializer));
            _nameInfoDeserializer = nameInfoDeserializer ?? throw new ArgumentNullException(nameof(nameInfoDeserializer));
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

            MeetingNameInfo nameInfo = _nameInfoDeserializer.JsonDeserialize<MeetingNameInfo>(meeting.MeetingNameJson);
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
