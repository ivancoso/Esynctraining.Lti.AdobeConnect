using System;
using EdugameCloud.Core;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // MeetingTitle
    internal sealed class StraightMeetingNameFormatter : IMeetingNameFormatter
    {
        private readonly IJsonSerializer _nameInfoSerializer;


        public string FormatName { get { return "[Meeting Title]"; } }


        public StraightMeetingNameFormatter(IJsonSerializer nameInfoSerializer)
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

            return meeting.Name.TruncateIfMoreThen(60);
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

            return lmsMeetingTitle.TruncateIfMoreThen(60);
        }

    }

}
