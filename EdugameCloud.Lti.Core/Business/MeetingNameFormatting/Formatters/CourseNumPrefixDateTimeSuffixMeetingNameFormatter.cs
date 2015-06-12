using System;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters
{
    // CourseNum: MeetingTitle (mm/dd/yy)
    internal sealed class CourseNumPrefixDateTimeSuffixMeetingNameFormatter : IMeetingNameFormatter
    {
        public string FormatName { get { return "CourseNum: MeetingTitle (mm/dd/yy)"; } }


        public string BuildName(MeetingDTO meeting, LtiParamDTO param, string courseId)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (param == null)
                throw new ArgumentNullException("param");
            if (courseId == null)
                throw new ArgumentNullException("courseId");

            int extraDataLength = ": ".Length + " (MM/dd/yy)".Length + param.context_label.Length;

            return string.Format("{0}: {1} ({2})", param.context_label, meeting.name.TruncateIfMoreThen(60 - extraDataLength), DateTime.Today.ToString("MM/dd/yy"));
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

            int extraDataLength = ": ".Length + " (MM/dd/yy)".Length + ((string)nameInfo.courseNum).Length;

            return string.Format("{0}: {1} ({2})", (string)nameInfo.courseNum, lmsMeetingTitle.TruncateIfMoreThen(60 - extraDataLength), (string)nameInfo.date);
        }

    }

}
