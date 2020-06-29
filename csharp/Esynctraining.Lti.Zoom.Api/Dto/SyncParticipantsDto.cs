using System.Collections.Generic;
using Esynctraining.Lti.Zoom.Common.Dto;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class SyncParticipantsDto
    {
        public List<RegistrantDto> MeetingRegistants { get; set; }
        public List<LmsAvailableUserDto> LmsAvailableUsers { get; set; }
    }
}
