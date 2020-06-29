using System.Runtime.Serialization;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class SeminarLicenseDto : SeminarLicenseDto<SeminarDto>
    {
        [DataMember]
        public bool CanAddSeminars { get; set; }
    }

}
