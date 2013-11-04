namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The telephony profile parser.
    /// </summary>
    internal static class TelephonyProfileParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Info.</returns>
        public static TelephonyProfile Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new TelephonyProfile
            {
                ProfileId = xml.SelectAttributeValue("profile-id"),
                ProviderId = xml.SelectAttributeValue("provider-id"),
                ProfileStatus = xml.SelectAttributeValue("profile-status"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                AdaptorId = xml.SelectSingleNodeValue("adaptor-id/text()"),
                ProfileName = xml.SelectSingleNodeValue("profile-name/text()"),
            };
        }
    }
}
