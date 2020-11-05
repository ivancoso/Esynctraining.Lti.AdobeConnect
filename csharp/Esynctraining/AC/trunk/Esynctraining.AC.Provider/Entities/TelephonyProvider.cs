namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    [XmlRoot("provider")]
    public class TelephonyProvider
    {
        public static class ProviderTypes
        {
            public static readonly string Integrated = "integrated";
            public static readonly string UserConfigured = "user-conf";
        }

        public static class ProviderStatuses
        {
            public static readonly string Enabled = "enabled";
            public static readonly string Disabled = "disabled";
        }

        [XmlElement("class-name")]
        public string ClassName { get; set; }

        [XmlElement("adaptor-id")]
        public string AdaptorId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("provider-status")]
        public string ProviderStatus { get; set; }
        
        [XmlAttribute("provider-id")]
        public string ProviderId { get; set; }

        [XmlAttribute("provider-type")]
        public string ProviderType { get; set; }

        [XmlAttribute("acl-id")]
        public string AclId { get; set; }

    }

}