using EdugameCloud.Lti.DTO;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class EditSeminarDto : MeetingDTOInput
    {
        [Required]
        [DataMember]
        public string SeminarLicenseId { get; set; }

    }

}
