namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    public class PermissionInfo : PermissionBase
    {
        [XmlAttribute("permission-id")]
        public PermissionId PermissionId { get; set; }

        public string PermissionStringValue { get; set; }

    }

}
