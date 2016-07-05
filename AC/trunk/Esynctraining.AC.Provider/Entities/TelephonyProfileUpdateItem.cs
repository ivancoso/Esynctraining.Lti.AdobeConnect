namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;
    using Attributes;

    //http://help.adobe.com/en_US/connect/8.0/webservices/WSc0dcc5922abc44bd571464d2127da61dbfe-8000.html#WSc6aa55b6edbd9ad2-3230cb5d126e9100633-7ffe
    //https://helpx.adobe.com/adobe-connect/webservices/topics/using-the-telephony-xml-api.html
    //https://helpx.adobe.com/adobe-connect/webservices/telephony-profile-update.html
    [Serializable]
    [XmlRoot("profile")]
    public class TelephonyProfileUpdateItem
    {
        /// <summary>
        /// Conference number associated with this profile. If you provide a value, any existing conference numbers for this profile are deleted.
        /// </summary>
        [XmlElement("conf-number")]
        public string ConferenceNumber { get; set; }

        ///// <summary>
        ///// String that specifies the field whose value needs to be updated. If this value is specified, you must also specify a value for provider-id.
        ///// </summary>
        //[XmlElement("field-id")]
        //public string FieldId { get; set; }

        /// <summary>
        /// String specifying the country code (for example, UK) of the location to be updated.
        /// Required: Y if you specify a value for conf-number; otherwise N.
        /// </summary>
        [XmlElement("location")]
        public string Location { get; set; }

        /// <summary>
        /// User for which the profile is created or updated. If not specified, the principal ID of the user who is currently logged in is used.
        /// </summary>
        [XmlElement("principal-id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// The profile to be updated. If not specified, a new profile is created.
        /// </summary>
        [XmlElement("profile-id")]
        public string ProfileId { get; set; }

        /// <summary>
        /// Name of the profile being created or updated.
        /// Required: Y if you are creating a profile
        /// </summary>
        [XmlElement("profile-name")]
        public string ProfileName { get; set; }

        /// <summary>
        /// Status of the profile. Acceptable values are enabled and disabled. If you disable a profile, all of its associations with meetings are removed. If you are creating a new profile, the default value is enabled.
        /// Required: Y if you are updating a profile
        /// </summary>
        [XmlAttribute("profile-status")]
        public string ProfileStatus { get; set; }

        /// <summary>
        /// Telephony provider for the profile being created or updated.
        /// </summary>
        [XmlAttribute("provider-id")]
        public string ProviderId { get; set; }

        ///// <summary>
        ///// Specifies the value of the field-id.
        ///// Required: Y if field-id is specified; otherwise N
        ///// </summary>
        //[XmlAttribute("value")]
        //public string Value { get; set; }

        
        [Skip]
        public ITelephonyProfileProviderFields ProviderFields { get; set; }

    }

}