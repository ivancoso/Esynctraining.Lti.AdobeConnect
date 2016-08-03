namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    [XmlRoot("profile")]
    public class TelephonyProfile
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the adaptor id.
        /// </summary>
        [XmlElement("adaptor-id")]
        public string AdaptorId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile id.
        /// </summary>
        [XmlAttribute("profile-id")]
        public string ProfileId { get; set; }

        /// <summary>
        /// Gets or sets the profile name.
        /// </summary>
        [XmlElement("profile-name")]
        public string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets the profile status.
        /// </summary>
        [XmlAttribute("profile-status")]
        public string ProfileStatus { get; set; }

        /// <summary>
        /// Gets or sets the provider id.
        /// </summary>
        [XmlAttribute("provider-id")]
        public string ProviderId { get; set; }

        #endregion

    }

}