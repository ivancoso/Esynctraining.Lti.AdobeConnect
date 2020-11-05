using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Meeting.Dto
{
    [DataContract]
    public class TemplateDto
    {
        [Required]
        [DataMember]
        public string Id { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

    }

}