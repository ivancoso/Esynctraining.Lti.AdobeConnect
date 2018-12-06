using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // MeetingTitle
    internal sealed class StraightMeetingNameFormatter : IMeetingNameFormatter
    {
        private readonly IJsonSerializer _nameInfoSerializer;
        private readonly IJsonDeserializer _nameInfoDeserializer;

        public string FormatName => "[Meeting Title]";


        public StraightMeetingNameFormatter(IJsonSerializer nameInfoSerializer, IJsonDeserializer nameInfoDeserializer)
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

            return meeting.Name.TruncateIfMoreThen(60);
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

            return lmsMeetingTitle.TruncateIfMoreThen(60);
        }

    }

}
