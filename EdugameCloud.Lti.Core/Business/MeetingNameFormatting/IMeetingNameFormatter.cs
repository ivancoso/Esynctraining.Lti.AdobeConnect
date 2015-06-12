using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting
{
    public interface IMeetingNameFormatter
    {
        string FormatName { get; }

        string BuildName(MeetingDTO meeting, LtiParamDTO param, string courseId);

        string UpdateName(LmsCourseMeeting meeting, string lmsMeetingTitle);

    }

}
