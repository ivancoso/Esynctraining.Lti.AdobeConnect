using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Seminar.Dto
{
    [DataContract]
    public class SeminarLicenseDto<TRoom>
    {
        [Required]
        [DataMember]
        public string Id { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [Required]
        [DataMember]
        public TRoom[] Rooms { get; set; }

    }

}