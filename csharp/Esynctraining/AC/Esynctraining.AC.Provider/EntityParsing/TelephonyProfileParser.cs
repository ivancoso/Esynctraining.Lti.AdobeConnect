namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The telephony profile parser.
    /// </summary>
    internal static class TelephonyProfileParser
    {
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

    internal static class TelephonyProfileFieldsParser
    {
        public static IDictionary<string, string> Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            var result = new Dictionary<string, string>(xml.ChildNodes.Count);
            foreach (XmlNode node in xml.ChildNodes)
            {
                result.Add(node.Name, node.SelectSingleNodeValue("text()"));
            }
            return result;
        }
    }
}
