namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("provider")]
    public class TelephonyProvider
    {
        public static class ProviderTypes
        {
            public static readonly string Integrated = "integrated";
            public static readonly string UserConfigured = "user-conf";
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