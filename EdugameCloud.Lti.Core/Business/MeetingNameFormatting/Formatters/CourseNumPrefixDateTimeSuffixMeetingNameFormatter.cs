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
        public string FormatName { get { return "[Course Label]: [Meeting Title] - (MM/DD/YY)"; } }


        public string BuildName(MeetingDTO meeting, LtiParamDTO param, string courseId)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (param == null)
                throw new ArgumentNullException("param");
            if (courseId == null)
                throw new ArgumentNullException("courseId");

            if (meeting.GetMeetingType() == LmsMeetingType.OfficeHours)
                return meeting.name.TruncateIfMoreThen(60);

            int extraDataLength = param.context_label.Length + ": ".Length + " - (MM/dd/yy)".Length;
            if (extraDataLength > 52)
                throw new WarningMessageException("Can't generate Adobe Connect meeting name. Course Label is too long.");

            return string.Format("{0}: {1} - ({2})", param.context_label, meeting.name.TruncateIfMoreThen(60 - extraDataLength), DateTime.Today.ToString("MM/dd/yy"));
        }

        public string UpdateName(LmsCourseMeeting meeting, string lmsMeetingTitle)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (string.IsNullOrWhiteSpace(lmsMeetingTitle))
                throw new ArgumentException("Meeting Title is required", "lmsMeetingTitle");

            MeetingNameInfo nameInfo = JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            nameInfo.meetingName = lmsMeetingTitle;
            meeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
                return lmsMeetingTitle.TruncateIfMoreThen(60);

            int extraDataLength = ((string)nameInfo.courseNum).Length + ": ".Length + " - (MM/dd/yy)".Length;
            if (extraDataLength > 52)
                throw new WarningMessageException("Can't generate Adobe Connect meeting name. Course Label is too long.");

            return string.Format("{0}: {1} - ({2})", (string)nameInfo.courseNum, lmsMeetingTitle.TruncateIfMoreThen(60 - extraDataLength), (string)nameInfo.date);
        }

    }

}
