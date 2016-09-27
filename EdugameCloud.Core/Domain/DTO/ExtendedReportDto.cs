using System.Collections.Generic;
using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.DTO
{
    public class ExtendedReportDto
    {
        public SubModuleItemType SubModuleItemType { get; set; }
        public string Name { get; set; }

        public IEnumerable<ExtendedReportResultDto> ReportResults { get; set; }

        public IEnumerable<Question> Questions { get; set; }
    }
}