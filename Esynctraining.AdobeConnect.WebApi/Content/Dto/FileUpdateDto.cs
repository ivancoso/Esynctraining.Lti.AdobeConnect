using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.WebApi.Content.Dto
{
    [DataContract]
    public class FileUpdateDto
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

    }

}
