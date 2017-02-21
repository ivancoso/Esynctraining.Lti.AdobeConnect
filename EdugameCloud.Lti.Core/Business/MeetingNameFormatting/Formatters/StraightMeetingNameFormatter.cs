using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // MeetingTitle
    internal sealed class StraightMeetingNameFormatter : IMeetingNameFormatter
    {
        public string FormatName { get { return "[Meeting Title]"; } }


        public string BuildName(MeetingDTO meeting, LtiParamDTO param, string courseId)
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

            MeetingNameInfo nameInfo = JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            nameInfo.meetingName = lmsMeetingTitle;
            meeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);

            return lmsMeetingTitle.TruncateIfMoreThen(60);
        }

    }

}
