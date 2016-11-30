using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Meeting.Dto
{
    [DataContract]
    public class TemplateDto
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

    }

}