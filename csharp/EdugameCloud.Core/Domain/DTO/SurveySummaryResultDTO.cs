using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{

    [DataContract]
    public class SurveySummaryResultDTO
    {
        [DataMember(IsRequired = true)]
        public int acSessionId { get; set; }

        [DataMember(IsRequired = true)]
        public int surveyId { get; set; }

        [DataMember(IsRequired = true)]
        public int companyId { get; set; }

        [DataMember(IsRequired = true)]
        public SurveyResultDTO[] surveyResults { get; set; }
    }
}
