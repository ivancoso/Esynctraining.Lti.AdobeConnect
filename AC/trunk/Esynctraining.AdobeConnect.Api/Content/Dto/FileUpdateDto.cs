using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Content.Dto
{
    [DataContract]
    public class FileUpdateDto
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        [Required]
        public string Name { get; set; }

    }

}
