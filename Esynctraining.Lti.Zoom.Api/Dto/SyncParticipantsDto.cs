using Esynctraining.Lti.Lms.Common.Dto;
using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class SyncParticipantsDto
    {
        public List<RegistrantDto> MeetingRegistants { get; set; }
        public List<LmsUserDTO> LmsAvailableUsers { get; set; }
    }
}
