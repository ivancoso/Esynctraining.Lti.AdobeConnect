using System.Collections.Generic;
using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.DTO
{
    public class ExtendedReportDto
    {
        public string QuizName { get; set; }

        public IEnumerable<ExtendedReportResultDto> QuizResults { get; set; }

        public IEnumerable<Question> Questions { get; set; }
    }
}