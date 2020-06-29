using Esynctraining.Lti.Zoom.Common.Dto.Sessions;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class MeetingSessionConverter
    {
        public static MeetingSessionDto ConvertFromEntity(LmsMeetingSession entity)
        {
            var result = new MeetingSessionDto
            {
                Id = entity.Id,
                Name = entity.Name,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Summary = entity.Summary,
            };

            return result;
        }
    }
}