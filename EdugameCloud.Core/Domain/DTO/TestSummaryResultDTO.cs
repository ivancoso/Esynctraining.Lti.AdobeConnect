using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{

    [DataContract]
    public class TestSummaryResultDTO
    {
        [DataMember(IsRequired = true)]
        public int acSessionId { get; set; }

        [DataMember(IsRequired = true)]
        public int companyId { get; set; }

        [DataMember(IsRequired = true)]
        public int testId { get; set; }

        [DataMember(IsRequired = true)]
        public TestResultDTO[] testResults { get; set; }
    }
}
