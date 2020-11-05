using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    public class SeminarSessionScoUpdateItem
    {
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        [XmlAttribute("source-sco-id")]
        public string SourceScoId { get; set; }

        [XmlAttribute("parent-acl-id")]
        public string ParentAclId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("date-begin")]
        public string DateBegin { get; set; }

        [XmlElement("date-end")]
        public string DateEnd { get; set; }

    }

}
