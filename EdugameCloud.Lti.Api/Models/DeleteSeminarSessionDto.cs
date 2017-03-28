using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class DeleteSeminarSessionDto
    {
        [Required]
        [DataMember]
        public string SeminarSessionId { get; set; }

    }

}
