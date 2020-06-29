using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting
{
    public interface IMeetingNameFormatter
    {
        string FormatName { get; }

        string BuildName(MeetingDTOLtiBase<MeetingSessionDTO> meeting, ILtiParam param, string courseId);

        string UpdateName(LmsCourseMeeting meeting, string lmsMeetingTitle);

    }

}
