using System.Runtime.Serialization;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;

//TODO: Esynctraining.AdobeConnect.WebApi.Seminar.Dto
namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class SeminarDto : MeetingDTOLtiBase<SeminarSessionDto>
    {
        // NOTE: EXTRA!!
        [DataMember(Name = "seminarLicenseId")]
        public string SeminarLicenseId { get; set; }

    }

}
