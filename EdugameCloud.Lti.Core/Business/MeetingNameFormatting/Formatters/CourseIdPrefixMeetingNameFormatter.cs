using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // [course_id] MeetingTitle
    internal sealed class CourseIdPrefixMeetingNameFormatter : IMeetingNameFormatter
    {
        public string FormatName { get { return "[ID] : MeetingTitle"; } }


        public string BuildName(MeetingDTO meeting, LtiParamDTO param, string courseId)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (param == null)
                throw new ArgumentNullException("param");
            if (courseId == null)
                throw new ArgumentNullException("courseId");

            return string.Format("[{0}] {1}", courseId, meeting.name).TruncateIfMoreThen(60);
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

            return string.Format("[{0}] {1}", (string)nameInfo.courseId, lmsMeetingTitle).TruncateIfMoreThen(60);
        }

    }

}
