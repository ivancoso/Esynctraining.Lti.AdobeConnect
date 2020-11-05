using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    //<field permission-id="manage" object-type="object-type-read-only" field-id="x-984625950" account-id="965886535" display-seq="5" field-type="text" is-primary="false" is-required="false">
    //    <comments>eSyncTraining MP4 Service Internal Use</comments>
    //    <name>esync-mp4-result</name>
    //</field>
    public class CustomField
    {
        public enum CustomFieldType
        {
            text,
            textarea,
            password,
        }

        // NOTE: always == "manage"
        [XmlAttribute("permission-id")]
        public PermissionId PermissionId { get; set; }

        [XmlAttribute("object-type")]
        public ObjectType ObjectType { get; set; }

        [XmlAttribute("field-id")]
        public string FieldId { get; set; }

        [XmlAttribute("account-id")]
        public string AccountId { get; set; }

        [XmlAttribute("display-seq")]
        public int DisplaySeq { get; set; }

        [XmlAttribute("field-type")]
        public CustomFieldType FieldType { get; set; }

        [XmlAttribute("is-primary")]
        public bool IsPrimary { get; set; }

        [XmlAttribute("is-required")]
        public bool IsRequired { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("comments")]
        public string Comments { get; set; }


        public CustomField()
        {
            PermissionId = PermissionId.manage;
            FieldType = CustomFieldType.text;
        }

    }

}
