using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.WebApi.Content.Dto
{
    [DataContract]
    public class FolderDto
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "folder_id")]
        public string FolderId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "sco_id")]
        public string ScoId { get; set; }

    }

}
