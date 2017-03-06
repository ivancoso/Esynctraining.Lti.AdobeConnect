using System.Collections.Generic;
using System.Runtime.Serialization;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;

//TODO: Esynctraining.AdobeConnect.WebApi.Seminar.Dto
namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class SeminarDto : MeetingDTOLtiBase<SeminarSessionDto>
    {
        [DataMember]
        public string SeminarLicenseId { get; set; }

    }

    [DataContract]
    public class SeminarAndLmsUsersDTO
    {
        [DataMember]
        public SeminarDto Meeting { get; set; }

        [DataMember]
        public IEnumerable<LmsUserDTO> LmsUsers { get; set; }

    }

}
