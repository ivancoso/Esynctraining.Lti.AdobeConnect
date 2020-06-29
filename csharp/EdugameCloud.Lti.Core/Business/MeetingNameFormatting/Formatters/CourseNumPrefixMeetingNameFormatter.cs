using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // CourseNum: MeetingTitle
    internal sealed class CourseNumPrefixMeetingNameFormatter : IMeetingNameFormatter
    {
        private readonly IJsonSerializer _nameInfoSerializer;
        private readonly IJsonDeserializer _nameInfoDeserializer;


        public string FormatName => "[Course Label]: [Meeting Title]";


        public CourseNumPrefixMeetingNameFormatter(IJsonSerializer nameInfoSerializer, IJsonDeserializer nameInfoDeserializer)
        {
            _nameInfoSerializer = nameInfoSerializer ?? throw new ArgumentNullException(nameof(nameInfoSerializer));
            _nameInfoDeserializer = nameInfoDeserializer ?? throw new ArgumentNullException(nameof(nameInfoDeserializer));
        }


        public string BuildName(MeetingDTOLtiBase<MeetingSessionDTO> meeting, ILtiParam param, string courseId)
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

            MeetingNameInfo nameInfo = _nameInfoDeserializer.JsonDeserialize<MeetingNameInfo>(meeting.MeetingNameJson);
            nameInfo.meetingName = lmsMeetingTitle;
            meeting.MeetingNameJson = _nameInfoSerializer.JsonSerialize(nameInfo);

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
                return lmsMeetingTitle.TruncateIfMoreThen(60);

            return string.Format("{0}: {1}", (string)nameInfo.courseNum, lmsMeetingTitle).TruncateIfMoreThen(60);
        }

    }

}
