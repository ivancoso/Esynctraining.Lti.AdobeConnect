using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // CourseNum: MeetingTitle - ID
    internal sealed class CourseNumPrefixIdSuffixMeetingNameFormatter : IMeetingNameFormatter
    {
        public string FormatName { get { return "[Course Label]: [Meeting Title] - [ID]"; } }


        public string BuildName(MeetingDTO meeting, LtiParamDTO param, string courseId)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (param == null)
                throw new ArgumentNullException("param");
            if (courseId == null)
                throw new ArgumentNullException("courseId");

            int extraDataLength = ": ".Length + " - ".Length + courseId.Length;

            return string.Format("{0}: {1} - {2}", param.context_label, meeting.name.TruncateIfMoreThen(60 - extraDataLength), courseId);
        }

        public string UpdateName(LmsCourseMeeting meeting, string lmsMeetingTitle)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (string.IsNullOrWhiteSpace(lmsMeetingTitle))
                throw new ArgumentException("Meeting Title is required", "lmsMeetingTitle");

            dynamic nameInfo = JObject.Parse(meeting.MeetingNameJson);
            nameInfo.meetingName = lmsMeetingTitle;
            meeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);

            int extraDataLength = ": ".Length + " - ".Length + ((string)nameInfo.courseId).Length;

            return string.Format("{0}: {1} - {2}", (string)nameInfo.courseNum, lmsMeetingTitle.TruncateIfMoreThen(60 - extraDataLength), (string)nameInfo.courseId);
        }

    }

}
