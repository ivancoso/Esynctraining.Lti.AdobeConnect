namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;
    
    public abstract class PermissionBase
    {
        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        [XmlAttribute("has-children")]
        public bool HasChildren { get; set; }

        [XmlAttribute("is-primary")]
        public bool IsPrimary { get; set; }
        
        [XmlElement("login")]
        public string Login { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

    }

}
