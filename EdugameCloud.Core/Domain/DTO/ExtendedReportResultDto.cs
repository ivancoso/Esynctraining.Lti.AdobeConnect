using System.Collections.Generic;

namespace EdugameCloud.Core.Domain.DTO
{
    public class ExtendedReportResultDto
    {
        public ExtendedReportResultDto()
        {
            Results = new List<QuestionResultDto>();
        }

        public int Id { get; set; }

        public string ParticipantName { get; set; }

        public IList<QuestionResultDto> Results { get; set; }
    }
}