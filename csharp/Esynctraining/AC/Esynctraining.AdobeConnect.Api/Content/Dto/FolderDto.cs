using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Content.Dto
{
    [DataContract]
    public class FolderDto
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        [Required]
        public string FolderId { get; set; }

        [DataMember]
        [Required]
        public string Name { get; set; }

        [DataMember]
        public string ScoId { get; set; }

    }

}
